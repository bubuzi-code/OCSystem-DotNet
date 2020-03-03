using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OnlyChain.Network {
    public class UniversalSocket {
        //static readonly int ProcessorCount = Environment.ProcessorCount;

        //public readonly ref struct ReceiveFromEventArgs {
        //    public readonly int SocketId;
        //    public readonly ReadOnlySpan<byte> Data;
        //    public readonly EndPoint RemoteEndPont;

        //    public ReceiveFromEventArgs(int socketId, ReadOnlySpan<byte> data, EndPoint remoteEndPont) {
        //        SocketId = socketId;
        //        Data = data;
        //        RemoteEndPont = remoteEndPont;
        //    }
        //}




        //private sealed class SubSocket : IAsyncDisposable {
        //    public readonly int SocketId;
        //    public readonly Socket socket;
        //    public readonly CancellationToken cancellationToken;
        //    private readonly List<Task> tasks = new List<Task>();
        //    private int tps = 0;
        //    private readonly Timer tpsTask;

        //    public delegate void SubReceiveFromEventHandler(SubSocket socket, in ReceiveFromEventArgs args);

        //    public event SubReceiveFromEventHandler ReceiveFrom;

        //    public int TPS { get; private set; }

        //    public SubSocket(Socket socket, CancellationToken cancellationToken) {
        //        this.socket = socket;
        //        this.cancellationToken = cancellationToken;

        //        tasks.Add(Task.Factory.StartNew(StartReceive, TaskCreationOptions.LongRunning));

        //        tpsTask = new Timer(delegate {
        //            if (cancellationToken.IsCancellationRequested) return;
        //            TPS = tps;
        //            Interlocked.Exchange(ref tps, 0);
        //            int concurrency = TPS / 10_0000 + 1;
        //            if (concurrency > tasks.Count && concurrency <= ProcessorCount) {
        //                tasks.Add(Task.Factory.StartNew(StartReceive, TaskCreationOptions.LongRunning));
        //            }
        //        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        //    }

        //    private async void StartReceive() {
        //        EndPoint remoteEP = socket.AddressFamily switch
        //        {
        //            AddressFamily.InterNetwork => new IPEndPoint(IPAddress.None, 0),
        //            AddressFamily.InterNetworkV6 => new IPEndPoint(IPAddress.IPv6None, 0),
        //            _ => null
        //        };
        //        var buffer = new byte[4096];
        //        while (!cancellationToken.IsCancellationRequested) {
        //            try {
        //                var result = await socket.ReceiveFromAsync(buffer, SocketFlags.None, remoteEP);
        //                Interlocked.Increment(ref tps);
        //                ReceiveFrom?.Invoke(this, new ReceiveFromEventArgs(SocketId, buffer.AsSpan(0, result.ReceivedBytes), result.RemoteEndPoint));
        //            } catch { }
        //        }
        //    }

        //    private bool isDisposed = false;

        //    async ValueTask Dispose(bool disposing) {
        //        if (!isDisposed) {
        //            await tpsTask.DisposeAsync();
        //            try {
        //                socket.Shutdown(SocketShutdown.Both);
        //            } catch { }
        //            foreach (var task in tasks) await task;
        //            if (disposing) {
        //                socket.Dispose();
        //                foreach (var task in tasks) task.Dispose();
        //                GC.SuppressFinalize(this);
        //            }
        //            isDisposed = true;
        //        }
        //    }

        //    ~SubSocket() {
        //        Dispose(false).AsTask().Wait();
        //    }

        //    public ValueTask DisposeAsync() => Dispose(true);
        //}

        private readonly Socket v4socket, v6socket;


    }
}
