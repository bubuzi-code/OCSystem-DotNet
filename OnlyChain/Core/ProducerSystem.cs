#nullable enable

using OnlyChain.Network;
using OnlyChain.Secp256k1;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlyChain.Core {
    public sealed class ProducerSystem : IDisposable {
        static readonly TimeSpan TimeSpanRange = TimeSpan.FromMinutes(3); // 区块时间戳超过当前系统时间3分钟之外认为无效
        const int NextBlockMilliseconds = 1500; // 允许跨见证人的间隔时间
        const byte ProducerBlockChipPrefix = 1;
        const byte OtherBlockChipPrefix = 2;
        const byte PrecommitVoteTypePrefix = 3;
        const byte CommitVoteTypePrefix = 4;

        private readonly IClient client;
        private readonly BlockChainSystem system;
        private readonly SuperNodeClient superNodeClient;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly TransactionPool transactionPool = new TransactionPool(); // 交易池
        private readonly Blacklist<Hash<Size256>> blacklistTransactions = new Blacklist<Hash<Size256>>(Constants.BlacklistTxCount); // 交易黑名单
        private readonly ConcurrentDictionary<Hash<Size160>, BlockChipCollection> blockChipCollectionDict = new ConcurrentDictionary<Hash<Size160>, BlockChipCollection>();
        private readonly ConcurrentBag<CommitVote> precommitVotes = new ConcurrentBag<CommitVote>();
        private readonly ConcurrentBag<CommitVote> commitVotes = new ConcurrentBag<CommitVote>();
        private readonly Stopwatch roundTimer = new Stopwatch();
        private readonly byte[] privateKey;



        public ProducerSystem(IClient client, SuperNodeClient superNodeClient, byte[] privateKey) {
            this.client = client;
            system = client.System;
            this.superNodeClient = superNodeClient;
            this.privateKey = privateKey;

            system.StateTransferred += System_StateTransferred;
            superNodeClient.DataArrived += SuperNodeClient_DataArrived;
        }

        /// <summary>
        /// 根据当前出块人获取之后所有出块人的列表
        /// </summary>
        /// <param name="currentProducer"></param>
        /// <returns></returns>
        public IReadOnlyList<int>? GetNextProducers(Address currentProducer) {
            int index = system.ImmutableCampaignNodes.IndexOfKey(currentProducer);
            if (index < 0 || index >= Constants.MinProducerCount) return null;
            var result = new int[Constants.MinProducerCount];
            for (int i = 0; i < Constants.MinProducerCount; i++) {
                result[i] = (index + i + 1) % Constants.MinProducerCount;
            }
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<挂起>")]
        private void SuperNodeClient_DataArrived(object? sender, SuperNodeEventArgs e) {
            if (e.Data.IsEmpty) return;

            try {
                switch (e.Data.Span[0]) {
                    case ProducerBlockChipPrefix:
                        ProducerBlockChipArrived(BlockChip.Parse(e.Data.Span[1..]), e.SuperNode.PublicKey);
                        SuperNodeBroadcast(MakeBroadcastPacket(OtherBlockChipPrefix, e.Data.Span[1..]), e.SuperNode.PublicKey.ToAddress());
                        break;
                    case OtherBlockChipPrefix:
                        OtherBlockChipArrived(BlockChip.Parse(e.Data.Span[1..]));
                        break;
                    case PrecommitVoteTypePrefix:
                        var precommitVote = new CommitVote(e.Data.Span[1..]);
                        if (!precommitVote.IsPrecommit) return;
                        if (Ecdsa.Verify(e.SuperNode.PublicKey, precommitVote.Hash, precommitVote.Signature) is false) return;
                        PrecommitVoteArrived(precommitVote);
                        break;
                    case CommitVoteTypePrefix:
                        var commitVote = new CommitVote(e.Data.Span[1..]);
                        if (commitVote.IsPrecommit) return;
                        if (Ecdsa.Verify(e.SuperNode.PublicKey, commitVote.Hash, commitVote.Signature) is false) return;
                        CommitVoteArrived(commitVote);
                        break;
                }
            } catch {
            }
        }

        private void ProducerBlockChipArrived(BlockChip blockChip, PublicKey producerPublicKey) {
            Address address = producerPublicKey.ToAddress();
            int beginIndex = system.LastBlock.IndexInRound + 1;
            int indexRange = Math.Clamp((int)Math.Ceiling((double)roundTimer.ElapsedMilliseconds / NextBlockMilliseconds), 1, Constants.MinProducerCount);
            bool allowProduce = false;
            for (int i = 0; i < indexRange; i++) {
                if (system.ImmutableCampaignNodes.Keys[(beginIndex + i) % Constants.MinProducerCount] == address) {
                    allowProduce = true;
                    break;
                }
            }
            if (allowProduce is false) throw new InvalidBlockChipException();

            if (blockChipCollectionDict.TryAdd(blockChip.Id, new BlockChipCollection(blockChip, address, producerPublicKey)) is false) {
                throw new InvalidBlockChipException();
            }
        }

        private void OtherBlockChipArrived(BlockChip blockChip) {
            if (!blockChipCollectionDict.TryGetValue(blockChip.Id, out var blockChipCollection))
                throw new InvalidBlockChipException();

            blockChipCollection.Add(blockChip);
        }

        private void PrecommitVoteArrived(CommitVote precommitVote) {
            precommitVotes.Add(precommitVote);
        }

        private void CommitVoteArrived(CommitVote commitVote) {
            commitVotes.Add(commitVote);
        }

        private byte[] MakeBroadcastPacket(byte typePrefix, ReadOnlySpan<byte> chipData) {
            var buffer = new byte[1 + chipData.Length];
            buffer[0] = typePrefix;
            chipData.CopyTo(buffer.AsSpan(1));
            return buffer;
        }

        private async void SuperNodeBroadcast(ReadOnlyMemory<byte> data, Address beginAddress) {
            IReadOnlyList<int>? forwardIndexes = GetNextProducers(beginAddress);
            if (forwardIndexes is null) return;

            var tasks = new List<Task>();
            for (int i = 0; i < forwardIndexes.Count - 1; i++) {
                if (system.ImmutableCampaignNodes.Keys[forwardIndexes[i]] == client.Address) continue;

                SuperNode? superNode = system.ImmutableCampaignNodes.Values[forwardIndexes[i]];
                if (superNode is { } && superNode.Connected) {
                    tasks.Add(superNode.SendAsync(data, cancellationTokenSource.Token).AsTask());
                }
            }
            await Task.WhenAll(tasks.ToArray());
        }

        private void SuperNodeBroadcast(ReadOnlyMemory<byte> data) => SuperNodeBroadcast(data, client.Address);


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<挂起>")]
        private async Task<Block?> GetBlockAsync(int timeoutMilliseconds) {
            Stopwatch? timer = null;
            if (timeoutMilliseconds > 0) {
                timer = new Stopwatch();
                timer.Start();
            }

            try {
                while (true) {
                    foreach (var (id, chipCollection) in blockChipCollectionDict.ToArray()) {
                        if (chipCollection.CanRestore) {
                            try {
                                return await chipCollection.RestoreAsync();
                            } finally {
                                blockChipCollectionDict.Remove(id, out _);
                            }
                        }
                    }

                    if (timer is { } && timer.ElapsedMilliseconds >= timeoutMilliseconds) {
                        break;
                    }

                    await Task.Delay(1, cancellationTokenSource.Token);
                }
            } catch {
            }
            return null;
        }

        private Block? TryGetBlock() {
            foreach (var (id, chipCollection) in blockChipCollectionDict) {
                if (chipCollection.CanRestore) {
                    try {
                        return chipCollection.RestoreAsync().Result;
                    } finally {
                        blockChipCollectionDict.Remove(id, out _);
                    }
                }
            }
            return null;
        }

        private void System_StateTransferred(object? sender, SystemStateTransferredEventArgs e) {

        }



        public void EnterProduction() {
            // TODO: 连接其他超级节点


            StartLoop();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<挂起>")]
        private async void StartLoop() {
            roundTimer.Restart();

            Task Yield() => Task.Delay(1, cancellationTokenSource.Token);

            int maxVoteMissCount = Constants.MinProducerCount / 3;
            var precommitHashs = new Dictionary<Hash<Size256>, int>();
            var precommitTimer = new Stopwatch();

            while (cancellationTokenSource.IsCancellationRequested is false) {
                precommitHashs.Clear();
                int precommitNull = 0, precommitMiss = 0;
                int commitHash = 0, commitNull = 0;
                Block? block = null;
                bool validBlock = false;
                bool noProduce = false; // 是否不共识，而处于同步状态
                precommitTimer.Restart();

                try {
                SynchronizeState:
                    if (noProduce) {
                        // TODO: 同步区块

                        noProduce = false;
                        continue;
                    }

                    if (block is null) {
                        block = TryGetBlock();
                        validBlock = false;
                    }

                    if (block is { }) {
                        // TODO: 验证区块，修改 validBlock，投出precommit hash或precommit null
                        precommitTimer.Restart();

                    }

                PrecommitState:
                    Hash<Size256>? precommitHash = null;

                    foreach (var precommit in precommitVotes) {
                        if (precommit.HashPrevBlock == system.LastBlock.Hash) {
                            if (precommit.HashVote == Hash<Size256>.Empty) {
                                precommitNull++;
                                if (precommitNull >= Constants.MinProducerVoteAccpetCount) {
                                    // TODO: 转到commit投票阶段
                                    try { goto CommitState; } finally { await Yield(); }
                                }
                            } else {
                                precommitHashs[precommit.HashVote]++;
                                if (precommitHashs[precommit.HashVote] >= Constants.MinProducerVoteAccpetCount) {
                                    // TODO: 转到commit投票阶段
                                    precommitHash = precommit.HashVote;
                                    try { goto CommitState; } finally { await Yield(); }
                                }
                            }
                        } else {
                            precommitMiss++;
                            if (precommitMiss > maxVoteMissCount) {
                                // TODO: 跟其他超级节点不在同一频道，退出共识，进入同步状态
                                noProduce = true;
                                try { goto SynchronizeState; } finally { await Yield(); }
                            }
                        }
                    }

                    if (block is { }) {
                        if (precommitTimer.ElapsedMilliseconds > 500) {
                            try { goto CommitState; } finally { await Yield(); }
                        }
                        try { goto PrecommitState; } finally { await Yield(); }
                    } else {
                        try { goto SynchronizeState; } finally { await Yield(); }
                    }

                CommitState:
                    CommitVote myCommitVote;
                    if (block is { } && validBlock && precommitHash != null && precommitHash == block.Hash) {
                        myCommitVote = CommitVote.Commit(block.HashPrevBlock, block.Hash, privateKey);
                    } else {
                        myCommitVote = CommitVote.Commit(system.LastBlock.HashPrevBlock, Hash<Size256>.Empty, privateKey);
                    }
                    SuperNodeBroadcast(MakeBroadcastPacket(CommitVoteTypePrefix, myCommitVote.Serialize()));


                    var commitTimer = Stopwatch.StartNew();
                    while (commitTimer.ElapsedMilliseconds <= 500) {
                        foreach (var vote in commitVotes) {
                            if (vote.HashPrevBlock == system.LastBlock.Hash) {
                                if (precommitHash != null && vote.HashVote == precommitHash) {
                                    commitHash++;
                                    if (commitHash >= Constants.MinProducerVoteAccpetCount) {
                                        goto CommitBlock;
                                    }
                                } else if (vote.HashVote == Hash<Size256>.Empty) {
                                    commitNull++;
                                    if (commitNull >= Constants.MinProducerVoteAccpetCount) {
                                        goto NoCommit;
                                    }
                                }
                            }
                        }
                        await Yield();
                    }

                NoCommit:
                    continue;

                CommitBlock:
                    // TODO: 提交区块到System

                    ;

                    // 在未收满precommit null的时候，且precommit hash数量为0，那么继续等待区块和precommit
                    // 收满precommit null，转到commit投票阶段，投commit null
                    // 收满precommit hash但没有收到区块，转到commit投票阶段，投commit null
                    // 收到区块，验证通过，立刻投出precommit hash
                    // 收到区块，验证不通过，立刻投出precommit null



                } catch {
                    continue;
                }

                roundTimer.Restart();
            }
        }

        /// <summary>
        /// TODO: 生产区块
        /// </summary>
        /// <returns></returns>
        public Block MakeBlock() {
            var newBlock = new Block() {
                Version = Constants.BlockVersion,
                Height = system.LastBlock.Height + 1,
                Timestamp = 0,
            };

            throw new NotImplementedException();
        }



        private bool isDisposed;

        private void Dispose(bool disposing) {
            if (!isDisposed) {
                cancellationTokenSource.Cancel();
                if (disposing) {

                }

                isDisposed = true;
            }
        }

        ~ProducerSystem() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
