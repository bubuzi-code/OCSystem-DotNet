#nullable enable

using OnlyChain.Network.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using OnlyChain.Core;
using System.IO;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading.Channels;
using System.Threading;
using System.Linq;

namespace OnlyChain.Network {
    [System.Diagnostics.DebuggerDisplay("{Address}, port: {Port}")]
    public sealed class Client : IClient, IAsyncDisposable {
        static readonly Random random = new Random();

        readonly TimeSpan refreshKBucketTimeSpan = TimeSpan.FromSeconds(20);
        static readonly TimeSpan broadcastTimeout = TimeSpan.FromHours(2);

        private readonly Timer refreshKBucketTimer;
        private readonly CancellationTokenSource closeCancelTokenSource = new CancellationTokenSource();
        private readonly Dictionary<Address, DateTime> broadcastIdRecord = new Dictionary<Address, DateTime>();
        private readonly UdpServer udpServer;
        private readonly KBucket kbucket;

        public string? NetworkPrefix { get; }
        public Address Address { get; }
        public int Port => udpServer.UdpPort;
        public KBucket Nodes => kbucket;
        public CancellationToken CloseCancellationToken => closeCancelTokenSource.Token;


        public event EventHandler<BroadcastEventArgs> ReceiveBroadcast = null!;

        public Client(Address address, int udpPort, string? networkPrefix = null, IPAddress? bindIP = null, IEnumerable<IPEndPoint>? seeds = null) {
            Address = address;
            NetworkPrefix = networkPrefix;

            udpServer = new UdpServer(this, udpPort, bindIP);
            kbucket = new KBucket(16, address, Ping);

            refreshKBucketTimeSpan = TimeSpan.FromSeconds(random.NextDouble() * 1 + 7);
            refreshKBucketTimer = new Timer(async delegate {
                var sw = new System.Diagnostics.Stopwatch();
                sw.Restart();
                if (closeCancelTokenSource.IsCancellationRequested) return;
                var randomAddress = Address.Random();
                var neighborNodes = kbucket.FindNode(randomAddress, 10);
                var tasks = Array.ConvertAll(neighborNodes, node => FindNode(randomAddress, kbucket.K, node.IPEndPoint, hasRemoteNode: false, refreshKBucket: true));

                var now = DateTime.Now;
                lock (broadcastIdRecord) {
                    var removeKeys = new List<Address>();
                    foreach (var (id, time) in broadcastIdRecord) {
                        if (now - time >= broadcastTimeout) removeKeys.Add(id);
                        else break;
                    }
                    foreach (var id in removeKeys) {
                        broadcastIdRecord.Remove(id);
                    }
                }

                foreach (var t in tasks) {
                    try { await t; } catch { }
                }
                sw.Stop();
                if (Address == "dbaaf68ee499766bdc548e324cdd204e3a563f2c") {
                    Console.WriteLine(sw.Elapsed);
                }
            }, null, refreshKBucketTimeSpan, refreshKBucketTimeSpan);


            Task? lookupTask = null;
            foreach (var seed in seeds ?? Enumerable.Empty<IPEndPoint>()) {
                var addresses = new SortedSet<Address>(Comparer<Address>.Create((a, b) => (a ^ Address).CompareTo(b ^ Address)));
                const int findCount = 10;

                async Task Find(IPEndPoint remote, bool hasRemoteNode) {
                    var nodes = await FindNode(Address, findCount, remote, hasRemoteNode, refreshKBucket: true).ConfigureAwait(true);
                    var lookupNodes = new List<Node>();
                    foreach (var node in nodes) {
                        if (node.Address == Address) continue;
                        lock (addresses) {
                            if (addresses.Count == findCount && (node.Address ^ Address) >= (addresses.Max ^ Address)) continue;
                            if (!addresses.Add(node.Address)) continue;
                            if (addresses.Count > findCount) addresses.Remove(addresses.Max);
                            lookupNodes.Add(node);
                        }
                    }

                    var findTasks = new List<Task>();
                    foreach (var node in lookupNodes) {
                        findTasks.Add(Find(node.IPEndPoint, false));
                    }
                    foreach (var t in findTasks) {
                        try { await t.ConfigureAwait(true); } catch { }
                    }
                }

                async void RandomSearch() {
                    try {
                        for (int i = 0; i < 5; i++) {
                            var randomAddress = Address.Random();
                            var nodes = kbucket.FindNode(randomAddress, findCount);
                            var tasks = new Task[nodes.Length];
                            for (int j = 0; j < nodes.Length; j++) {
                                tasks[j] = FindNode(randomAddress, findCount, nodes[j].IPEndPoint, hasRemoteNode: false, refreshKBucket: true);
                            }
                            foreach (var t in tasks) {
                                try { await t; } catch { }
                            }
                        }
                    } catch { }
                }

                if (lookupTask is null) {
                    lookupTask = Find(seed, true).ContinueWith(delegate { RandomSearch(); });
                } else {
                    lookupTask = lookupTask.ContinueWith(delegate { Find(seed, true).ContinueWith(delegate { RandomSearch(); }).ConfigureAwait(true); });
                }
            }
        }



        [CommandHandler("ping")]
        private BDict? PingHandle(RemoteRequest r) {
            if (r.Request["ping"] is BAddress { Value: var myAddress } && myAddress == Address) {
                return new BDict { ["pong"] = r.Address };
            }
            return null;
        }

        [CommandHandler("find_node")]
        private BDict? FindNodeHandle(RemoteRequest r) {
            int findCount = kbucket.K;
            if (!(r.Request["target"] is BAddress { Value: var target })) return null;
            if (r.Request["count"] is BUInt { Value: var count } && count > 0 && count < 50) findCount = (int)count;

            byte[] buffer = ArrayPool<byte>.Shared.Rent(findCount * 39);
            try {
                int len = 0;
                foreach (var node in kbucket.FindNode(target, findCount)) {
                    len += WriteNode(buffer.AsSpan(len), node);
                }
                return new BDict { ["nodes"] = buffer[0..len] };
            } finally {
                ArrayPool<byte>.Shared.Return(buffer);
                _ = kbucket.Add(new Node(r.Address, r.Remote), lookup: false);
            }
        }

        [CommandHandler("broadcast")]
        private async Task BroadcastHandle(RemoteRequest r) {
            if (ReceiveBroadcast is null) return;

            if (!(r.Request["id"] is BAddress { Value: var id })) return;
            if (!(r.Request["i"] is BUInt { Value: var ttl }) || ttl >= int.MaxValue) return;
            if (!(r.Request["msg"] is BBuffer { Buffer: byte[] message })) return;

            lock (broadcastIdRecord) {
                if (!broadcastIdRecord.TryAdd(id, DateTime.Now)) return;
                if (broadcastIdRecord.Count > 1000_0000) {
                    broadcastIdRecord.Remove(broadcastIdRecord.Keys.First());
                }
            }

            await Task.Yield(); // 处理广播消息可能要花很多时间

            var broadcast = new BroadcastEventArgs(new Node(r.Address, r.Remote), (int)ttl, message);
            ReceiveBroadcast(this, broadcast);
            if (broadcast.IsCancelForward) return;

            Broadcast(message, id, (int)ttl + 1);
        }

        private static int WriteNode(Span<byte> buffer, Node node) {
            node.Address.WriteToBytes(buffer);

            var surviveTimeSpan = DateTime.Now - node.RefreshTime;
            if (surviveTimeSpan < TimeSpan.Zero) surviveTimeSpan = TimeSpan.Zero; else if (surviveTimeSpan > KBucket.Timeout) surviveTimeSpan = KBucket.Timeout;
            var surviveSeconds = surviveTimeSpan.TotalSeconds + (1 << 51) - (1 << 51); // 根据IEEE 754，此操作可以实现四舍五入并保留1bit小数
            byte surviveTime = (byte)(surviveSeconds * 2);

            IPAddress ip = node.IPEndPoint.Address;
            if (ip.IsIPv4MappedToIPv6) {
                ip = ip.MapToIPv4();
            }

            if (ip.AddressFamily is AddressFamily.InterNetworkV6) {
                surviveTime |= 0x80;
            }
            buffer[Address.Size] = surviveTime;
            ip.TryWriteBytes(buffer.Slice(Address.Size + 1), out var ipBytes);
            BinaryPrimitives.WriteUInt16BigEndian(buffer.Slice(Address.Size + 1 + ipBytes, 2), (ushort)node.IPEndPoint.Port);
            return Address.Size + 3 + ipBytes;
        }

        private static int ReadNode(ReadOnlySpan<byte> buffer, out Node resultNode) {
            if (buffer.Length < 27) throw new ArgumentOutOfRangeException(nameof(buffer), "缓冲区太小");
            byte surviveTime = buffer[Address.Size];
            if (surviveTime >= 0x80 && buffer.Length < 39) throw new ArgumentOutOfRangeException(nameof(buffer), "缓冲区太小");

            var address = new Address(buffer.Slice(0, Address.Size));
            var surviveTimeSpan = TimeSpan.FromSeconds((surviveTime & 0x7f) / 2.0);
            var ipBytes = surviveTime >= 0x80 ? 16 : 4;
            var ip = new IPAddress(buffer.Slice(Address.Size + 1, ipBytes));
            var port = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(Address.Size + 1 + ipBytes, 2));
            resultNode = new Node(address, new IPEndPoint(ip, port), surviveTimeSpan);
            return Address.Size + 3 + ipBytes;
        }

        private async Task<bool> Ping(Node node) {
            try {
                var r = await udpServer.Request(new BDict { ["c"] = "ping", ["ping"] = node.Address }, node.IPEndPoint, cancellationToken: closeCancelTokenSource.Token);
                if (r.Address != node.Address) return false;
                if (!(r.Response["pong"] is BAddress { Value: var myAddress }) || myAddress != Address) return false;
                return true;
            } catch {
                return false;
            }
        }

        private async Task<Node[]> FindNode(Address target, int findCount, IPEndPoint remoteEP, bool hasRemoteNode, bool refreshKBucket = false, CancellationToken cancellationToken = default) {
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, closeCancelTokenSource.Token);
            var r = await udpServer.Request(new BDict { ["c"] = "find_node", ["target"] = target, ["count"] = (ulong)findCount }, remoteEP, cancellationToken: tokenSource.Token);
            if (!(r.Response["nodes"] is BBuffer { Buffer: byte[] buffer })) return Array.Empty<Node>();
            var nodes = new Dictionary<Address, Node>();
            for (int i = 0; i < buffer.Length;) {
                i += ReadNode(buffer.AsSpan(i), out var node);
                if (!nodes.ContainsKey(Address)) nodes.Add(node.Address, node);
            }
            if (hasRemoteNode) {
                nodes.Add(r.Address, new Node(r.Address, r.Remote));
            }

            if (refreshKBucket) {
                var tasks = new List<Task>();
                foreach (var node in nodes.Values) {
                    var task = kbucket.Add(node, lookup: true);
                    if (!task.IsCompleted) tasks.Add(task.AsTask());
                }
                foreach (var task in tasks) await task;
            }

            return nodes.Values.ToArray();
        }

        private void Broadcast(byte[] message, Hash<Size160> broadcastId, int ttl = 0) {
            if (ttl < 0 || ttl >= int.MaxValue) throw new ArgumentOutOfRangeException(nameof(ttl));

            var dict = new BDict { ["c"] = "broadcast", ["a"] = Address, ["msg"] = message, ["id"] = (Address)broadcastId, ["i"] = (ulong)ttl };
            var data = Bencode.Encode(dict, NetworkPrefix);
            var broadcastNodes = kbucket.FindNode(Address, kbucket.K, randomCount: 2);
            foreach (var node in broadcastNodes) {
                udpServer.Send(data, node.IPEndPoint);
            }
        }

        /// <summary>
        /// 在全网中查找目标地址的IP端口
        /// </summary>
        /// <param name="target"></param>
        /// <param name="nodePoolSize"></param>
        /// <returns></returns>
        public async ValueTask<Node?> Lookup(Address target, int nodePoolSize = 20) {
            var cancellationTokenSource = new CancellationTokenSource();
            var addresses = new SortedSet<Address>(Comparer<Address>.Create((a, b) => (a ^ target).CompareTo(b ^ target)));
            var result = new TaskCompletionSource<Node?>();

            async Task Find(IPEndPoint remote, bool hasRemoteNode) {
                var nodes = await FindNode(target, kbucket.K, remote, hasRemoteNode, refreshKBucket: false, cancellationTokenSource.Token);
                var lookupNodes = new List<Node>();
                foreach (var node in nodes) {
                    if (cancellationTokenSource.IsCancellationRequested) return;
                    if (node.Address == target) {
                        result.TrySetResult(node);
                        cancellationTokenSource.Cancel();
                        return;
                    }
                    lock (addresses) {
                        if (addresses.Count == nodePoolSize && (node.Address ^ target) >= (addresses.Max ^ target)) continue;
                        if (!addresses.Add(node.Address)) continue;
                        if (addresses.Count > nodePoolSize) addresses.Remove(addresses.Max);
                        lookupNodes.Add(node);
                    }
                }
                var findTasks = Array.ConvertAll(lookupNodes.ToArray(), node => Find(node.IPEndPoint, false));
                foreach (var t in findTasks) {
                    try { await t; } catch { }
                }
            }

            var tempNodes = kbucket.FindNode(target, kbucket.K);
            if (tempNodes.FirstOrDefault(n => n.Address == target) is Node r) return r;
            var tasks = Array.ConvertAll(tempNodes, node => Find(node.IPEndPoint, true));
            foreach (var t in tasks) {
                try { await t; } catch { }
            }
            result.TrySetResult(null);
            return await result.Task;
        }


        public void Broadcast(byte[] message) {
            var broadcastId = Hash<Size160>.Random();
            lock (broadcastIdRecord) {
                if (!broadcastIdRecord.TryAdd(broadcastId, DateTime.Now)) return;
            }
            Broadcast(message, broadcastId);
        }

        public bool IsDisposed { get; private set; } = false;

        async ValueTask Dispose(bool disposing) {
            if (!IsDisposed) {
                closeCancelTokenSource.Cancel();
                refreshKBucketTimer.Change(Timeout.Infinite, Timeout.Infinite);
                if (disposing) {
                    await udpServer.DisposeAsync();
                    await refreshKBucketTimer.DisposeAsync();
                    GC.SuppressFinalize(this);
                }
                IsDisposed = true;
            }
        }

        ~Client() {
            Dispose(false).AsTask().Wait();
        }

        public ValueTask DisposeAsync() => Dispose(true);
    }
}
