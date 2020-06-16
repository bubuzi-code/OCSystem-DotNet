#nullable enable

using OnlyChain.Database;
using OnlyChain.Model;
using OnlyChain.Network;
using OnlyChain.Secp256k1;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OnlyChain.Core {
    public sealed class BlockChainSystem : IDisposable {
        const int CacheHeight = 150; // 最大缓存数量

        static readonly Regex wordRegex = new Regex(@"^\w+$", RegexOptions.Compiled | RegexOptions.Singleline);

        private readonly IClient client;

        private readonly LevelDB immutableDatabase;
        private readonly UserDictionary userDatabse;
        private readonly Hashes<Hash<Size256>> blockHashes; // 所有区块的hash以及本地索引
        private readonly IndexedQueue<Block> cacheBlocks = new IndexedQueue<Block>(CacheHeight); // 缓存最近的区块

        private readonly List<Address> tempCampaignNodes = new List<Address>(); // 本轮临时新增的竞选节点
        private readonly Address[] sortedCampaignNodes = Array.Empty<Address>(); // 已确认的竞选排名

        private readonly Address[] blockProducers = new Address[Constants.MinProducerCount]; // 按得票排序的生产者

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly Channel<Block> blockChannel = Channel.CreateUnbounded<Block>(new UnboundedChannelOptions { SingleReader = true });

        public string ChainName { get; }
        public uint MaxHeight { get; private set; } = 0;

        public Block LastBlock { get; private set; }
        public SortedList<Address, SuperNode?> ImmutableCampaignNodes { get; private set; } = new SortedList<Address, SuperNode?>(); // 所有竞选节点与IP端口(TCP)，按得票数排序

        public event EventHandler<CampaignNodesChangedEventArgs>? CampaignNodesChanged;
        public event EventHandler<SystemStateTransferredEventArgs>? StateTransferred;



        public BlockChainSystem(IClient client, string chainName = "main") {
            this.client = client;

            ChainName = chainName ?? throw new ArgumentNullException(nameof(chainName));
            if (!wordRegex.IsMatch(chainName)) throw new ArgumentOutOfRangeException(nameof(chainName), "非法的链名");

            if (!Directory.Exists(chainName)) Directory.CreateDirectory(chainName);

            immutableDatabase = new LevelDB(Path.Combine(chainName, "block_chain"), new LevelDBOptions {
                CreateIfMissing = true,
                Cache = new LevelDBCache(1000),
            });
            userDatabse = new UserDictionary(Path.Combine(chainName, "block_chain.users"));
            blockHashes = new Hashes<Hash<Size256>>(Path.Combine(chainName, "block_chain.hash"));

            LastBlock = Block.GenesisBlock;
            // TODO: 更新创世区块状态

            _ = Task.Run(LoopReadBlock);
        }

        public PublicKey? GetPublicKey(Address address) => userDatabse.TryGetValue(address, out var value) ? value : null;

        private async void LoopReadBlock() {
            await foreach (var block in blockChannel.Reader.ReadAllAsync(cancellationTokenSource.Token)) {
                Put(block);
            }
        }

        /// <summary>
        /// 添加一个新的区块到区块链
        /// </summary>
        /// <param name="block"></param>
        /// <param name="verify">是否验证区块（自己生产的区块不验证）</param>
        private void Put(Block block) {
            try {
                // 添加一个新的高度的区块
                VerifyExecuteBlock(block);
                cacheBlocks.Enqueue(block, out _);
                StateTransition(block);

                int nativeBlockIndex = blockHashes.Add(block.Hash);
                // TODO: 把区块写入数据库

            } catch {

            }
        }

        private void ExecuteGenesisBlock(Block block) {
            block.PrecommitState = new BlockState {
                WorldState = new MerklePatriciaTree<Address, UserStatus, Hash<Size256>>(1),
                Transactions = new MerklePatriciaTree<Hash<Size256>, TransactionResult, Hash<Size256>>(0),

            };
        }

        /// <summary>
        /// 验证执行区块，并设置<see cref="Block.PrecommitState"/>字段（执行前<see cref="Block.PrecommitState"/>字段必须为null）。
        /// </summary>
        /// <param name="block">非创世区块。</param>
        unsafe public void VerifyExecuteBlock(Block block) {
            if (LastBlock.CommitState is null) throw new InvalidOperationException();

            if (LastBlock.Hash != block.HashPrevBlock || LastBlock.Height != block.Height - 1)
                throw new InvalidBlockException();

            block.PrecommitState = LastBlock.CommitState.NextNew();

            var verifyTasks = new List<Task>();
            var verifyCancellationTokenSource = new CancellationTokenSource();

            // 验证生产者
            if (!IsProducer(block.ProducerAddress)) throw new InvalidBlockException();

            // 验证签名，验证交易mpt
            verifyTasks.Add(Task.Run(delegate {
                bool verifyResult = Ecdsa.Verify(GetProducerPublicKey(block), block.HashSignHeader, block.Signature);
                if (verifyResult is false) throw new InvalidBlockException();
            }));

            // TODO: 验证出块时间

            // 并行验证交易签名
            int taskCount = Math.Max(Environment.ProcessorCount - 2, 1);
            var perCount = (int)Math.Ceiling(block.Transactions.Length / (double)taskCount);
            int counter = 0;
            while (counter < block.Transactions.Length) {
                int processCount = Math.Min(perCount, block.Transactions.Length - counter);
                int startIndex = counter;
                verifyTasks.Add(Task.Run(delegate {
                    for (int i = 0; i < processCount && !verifyCancellationTokenSource.IsCancellationRequested; i++) {
                        Transaction transaction = block.Transactions[i + startIndex];
                        Hash<Size256> hash = transaction.HashSignHeader;
                        bool verifyResult = Ecdsa.Verify(transaction.PublicKey, hash, transaction.Signature);
                        if (verifyResult is false) throw new InvalidBlockException();
                    }
                }));

                counter += processCount;
            }

            try {
                // 当区块跨过轮次，则统计票数
                TryStatisticsCampaignNodes(block);

                // 执行交易
                var txMPT = block.PrecommitState.Transactions;
                for (int i = 0; i < block.Transactions.Length; i++) {
                    Transaction tx = block.Transactions[i];
                    uint errorCode = 0;
                    try {
                        ExecuteTransaction(block, tx);
                    } catch (ExecuteTransactionException e) {
                        errorCode = e.ErrorCode;
                    }
                    txMPT.Add(tx.Hash, new TransactionResult(tx, errorCode));
                }

                // 验证交易MPT
                txMPT.ComputeHash(TransactionHashAlgorithm.Instance);
                if (block.HashTxMerkleRoot != txMPT.RootHash) throw new InvalidBlockException();

                // TODO: 出块奖励，设置世界MPT


                // 验证世界状态
                block.PrecommitState.WorldState.ComputeHash(UserStatusHashAlgorithm.Instance);
                if (block.HashWorldState != block.PrecommitState.WorldState.RootHash) throw new InvalidBlockException();

                // 等待所有交易签名验证完成
                Task.WaitAll(verifyTasks.ToArray());
            } catch {
                verifyCancellationTokenSource.Cancel();
                throw new InvalidBlockException();
            }
        }

        /// <remarks>
        /// <para>1、检查交易合法性</para>
        /// <para>2、执行交易（更新<paramref name="status"/>）</para>
        /// </remarks>
        /// <param name="block">一个未检查执行过的区块</param>
        /// <param name="status"></param>
        /// <param name="tx"></param>
        private void ExecuteTransaction(Block block, Transaction tx) {
            try {
                if ((ulong)tx.GasPrice is 0) goto Invalid; // 手续费不能为0
                if (tx.GasLimit < tx.BaseGasUsed) goto Invalid; // GasLimit比最低汽油费还低

                var mpt = block.PrecommitState!.WorldState;
                UserStatus fromStatus = mpt[tx.From];
                if (fromStatus.NextNonce != tx.Nonce) goto Invalid; // nonce不正确
                ulong minFee = checked(tx.BaseGasUsed * tx.GasPrice); // 最低手续费
                if (fromStatus.Balance < minFee) goto Invalid; // 不足以支付最低手续费

                UserStatus producerStatus = mpt[block.ProducerAddress]; // 生产者已验证
                fromStatus.Balance -= minFee; // 扣除最低手续费
                producerStatus.Balance += minFee;

                UserStatus rollbackFromStatus = fromStatus; // 保存当前状态，交易执行失败时回滚
                void Fail(TransactionErrorCode errorCode) {
                    mpt[block.ProducerAddress] = producerStatus;
                    SetFromStatus(rollbackFromStatus);
                    throw new ExecuteTransactionException((uint)errorCode);
                }

                void SetFromStatus(in UserStatus status) {
                    bool remove = (ulong)status.Balance is 0
                        && (ulong)status.VotePledge is 0
                        && (ulong)status.SuperPledge is 0
                        && status.Votes is 0;
                    if (remove) {
                        mpt.Remove(tx.From);
                    } else {
                        mpt[tx.From] = status;
                    }
                }

                if (fromStatus.Balance < tx.Value) Fail(TransactionErrorCode.InsufficientBalance); // 余额不足，但足够支付手续费
                fromStatus.Balance -= tx.Value;
                fromStatus.NextNonce++;

                switch (tx.AttachData) {
                    case null: // 普通交易
                        if (mpt.TryGetValue(tx.To, out UserStatus toStatus)) {
                            toStatus.Balance += tx.Value;
                            mpt[tx.To] = toStatus;
                        } else {
                            mpt[tx.To] = new UserStatus { Balance = tx.Value };
                        }
                        break;
                    case SuperPledgeData _: // 超级节点质押，本轮结束其他账户才能对自己投票
                        if ((ulong)fromStatus.SuperPledge is 0 && tx.Value < Constants.MinSuperPledgeCoin) Fail(TransactionErrorCode.PledgeTooLow); // 质押金额太小
                        if (fromStatus.SuperPledge >= Constants.MinSuperPledgeCoin && tx.Value < Constants.MinSuperPledgeIncrement) Fail(TransactionErrorCode.PledgeTooLow); // 质押金额太小
                        fromStatus.SuperPledge += tx.Value;
                        fromStatus.SuperPledgeTimestamp = block.Timestamp;
                        block.PrecommitState.TempCampaignNodes.Add(tx.From);
                        break;
                    case SuperRedemptionData _: // 超级节点赎回
                        if ((ulong)tx.Value != 0) Fail(TransactionErrorCode.ValueNotEqualToZero); // Value不为0
                        if (fromStatus.SuperPledge < Constants.MinSuperPledgeCoin) Fail(TransactionErrorCode.Unpledged); // 从未质押过
                        if (block.Timestamp - fromStatus.SuperPledgeTimestamp < Constants.YearSeconds) Fail(TransactionErrorCode.NotExpired); // 未到赎回期
                        fromStatus.Balance += fromStatus.SuperPledge;
                        fromStatus.SuperPledge = 0;
                        fromStatus.SuperPledgeTimestamp = 0;
                        break;
                    case VoteData vote: // 投票
                        if (vote.Round != block.Round) goto Invalid;
                        if (tx.Value < Constants.MinVotePledgeCoin) Fail(TransactionErrorCode.VoteTooLow);
                        if (vote.Addresses.Length != new HashSet<Address>(vote.Addresses).Count) Fail(TransactionErrorCode.DuplicateAddress); // 地址重复

                        var changedStatus = new Dictionary<Address, UserStatus>(60);

                        if (fromStatus.VoteAddresses is { }) {
                            foreach (var addr in fromStatus.VoteAddresses) {
                                UserStatus campaignStatus = mpt[addr];
                                campaignStatus.Votes -= fromStatus.VotePledge;
                                changedStatus[addr] = campaignStatus;
                            }
                        }

                        fromStatus.VotePledge += tx.Value;
                        fromStatus.VoteAddresses = vote.Addresses;
                        fromStatus.VotePledgeTimestamp = block.Timestamp;

                        foreach (Address addr in vote.Addresses) {
                            int index = Array.BinarySearch(block.PrecommitState.SortedCampaignNodes, addr, block.PrecommitState.CampaignComparer);
                            if (index < 0) Fail(TransactionErrorCode.Unpledged); // 投给非竞选节点
                            if (mpt.TryGetValue(addr, out UserStatus campaignStatus) is false) Fail(TransactionErrorCode.Unpledged);
                            if (IsCampaignNode(block, in campaignStatus)) Fail(TransactionErrorCode.Unpledged);
                            if (changedStatus.TryGetValue(addr, out UserStatus tempStatus)) campaignStatus = tempStatus;
                            campaignStatus.Votes += fromStatus.VotePledge;
                            changedStatus[addr] = campaignStatus;
                        }

                        foreach (var (addr, status) in changedStatus) mpt[addr] = status;
                        break;
                    case VoteRedemptionData _: // 投票质押赎回
                        if ((ulong)tx.Value != 0) Fail(TransactionErrorCode.ValueNotEqualToZero); // Value不为0
                        if (fromStatus.VoteAddresses is null) Fail(TransactionErrorCode.Unpledged); // 从未质押过
                        if (block.Timestamp - fromStatus.VotePledgeTimestamp < Constants.VotePledgeSeconds) Fail(TransactionErrorCode.NotExpired);

                        for (int i = 0; i < fromStatus.VoteAddresses!.Length; i++) {
                            Address addr = fromStatus.VoteAddresses[i];
                            UserStatus campaignStatus = mpt[addr];
                            campaignStatus.Votes -= fromStatus.VotePledge;
                            mpt[addr] = campaignStatus;
                        }

                        fromStatus.Balance += fromStatus.VotePledge;
                        fromStatus.VoteAddresses = null;
                        fromStatus.VotePledge = 0;
                        fromStatus.VotePledgeTimestamp = 0;
                        break;
                }

                mpt[block.ProducerAddress] = producerStatus;
                SetFromStatus(fromStatus);
                return;
            } catch { /*Invalid*/ }
        Invalid:
            throw new InvalidTransactionException(tx);
        }


        /// <summary>
        /// 是否拥有生产区块的权限
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool IsProducer(Address address) {
            int rank = ImmutableCampaignNodes.IndexOfKey(address);
            return 0 <= rank && rank < Constants.MinProducerCount;
        }

        /// <summary>
        /// 是否竞选节点
        /// </summary>
        /// <param name="block"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private static bool IsCampaignNode(Block block, in UserStatus status) {
            if (status.SuperPledge < Constants.MinSuperPledgeCoin) return false;
            if (block.Timestamp - status.SuperPledgeTimestamp >= Constants.YearSeconds) return false;
            return true;
        }

        private PublicKey GetProducerPublicKey(Block block) {
            return userDatabse[block.ProducerAddress];
        }

        /// <summary>
        /// 让系统状态在相邻的区块间转移
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void StateTransition(Block to) {
            Block from = LastBlock;
            LastBlock = to;

            var eventArgs = new SystemStateTransferredEventArgs(from, to);
            StateTransferred?.Invoke(this, eventArgs);
        }

        private void Forward(Block next) {

        }

        private void Rollback(Block prev) {

        }

        ///// <summary>
        ///// 新的不可逆区块
        ///// </summary>
        ///// <param name="block"></param>
        //private void Immutable(Block block) {
        //    if (ImmutableLastBlock.Round != block.Round) {
        //        var newCampaignNodes = new SortedList<Address, SuperNode?>(block.CampaignComparer);
        //        foreach (var addr in block.SortedCampaignNodes) newCampaignNodes.Add(addr, null);

        //        foreach (var (oldAddress, oldNode) in ImmutableCampaignNodes) {
        //            if (newCampaignNodes.ContainsKey(oldAddress)) {
        //                newCampaignNodes[oldAddress] = oldNode;
        //            } else {
        //                oldNode?.Dispose();
        //            }
        //        }

        //        ImmutableCampaignNodes = newCampaignNodes; // 更新竞选排名列表
        //    }

        //    ImmutableLastBlock = block;
        //}

        unsafe private Block? ReadBlockFromDatabase(Hash<Size256> key) {
            byte[]? blockByteData = immutableDatabase.Get(new ReadOnlySpan<byte>(&key, sizeof(Hash<Size256>)));
            if (blockByteData is null) return null;

            // TODO: 从blockByteData构建Block
            return null;
        }

        /// <summary>
        /// 统计票数并排名
        /// </summary>
        /// <param name="block"></param>
        private void TryStatisticsCampaignNodes(Block block) {
            if (LastBlock.Round == block.Round) return;
            Debug.Assert(LastBlock.CommitState is { });
            Debug.Assert(block.PrecommitState is { });

            block.PrecommitState.CampaignComparer = new CampaignComparer(LastBlock.CommitState.WorldState);
            Address[] allCampaignNodes = block.PrecommitState.SortedCampaignNodes.Concat(block.PrecommitState.TempCampaignNodes).Distinct().ToArray();
            Array.Sort(allCampaignNodes, block.PrecommitState.CampaignComparer);
            int removeCount = 0;
            for (int i = 0; i < allCampaignNodes.Length; i++) {
                ref readonly Address addr = ref allCampaignNodes[^(i + 1)];
                ref readonly UserStatus status = ref LastBlock.CommitState.WorldState.TryGetValue(addr);
                if (status.IsNull() || (ulong)status.SuperPledge is 0) {
                    removeCount++;
                } else {
                    break;
                }
            }
            if (removeCount != 0) {
                block.PrecommitState.SortedCampaignNodes = allCampaignNodes[..^removeCount];
            } else {
                block.PrecommitState.SortedCampaignNodes = allCampaignNodes;
            }
            block.PrecommitState.TempCampaignNodes = new List<Address>();
        }

        private bool disposedValue = false;

        void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    immutableDatabase.Dispose();
                    blockHashes.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);

        unsafe public bool ContainsKey(Hash<Size256> key)
            => blockHashes.ContainsKey(key);

        public bool TryGetValue(Hash<Size256> key, [MaybeNullWhen(false)] out Block value) {
            // TODO: 尝试从内存中读取区块

            value = ReadBlockFromDatabase(key);
            return value is { };
        }


        sealed class CampaignComparer : IComparer<Address> {
            private readonly MerklePatriciaTree<Address, UserStatus, Hash<Size256>> mpt;

            public CampaignComparer(MerklePatriciaTree<Address, UserStatus, Hash<Size256>> mpt) => this.mpt = mpt;

            unsafe public int Compare(Address x, Address y) {
                if (x == y) return 0;
                ref readonly UserStatus xValue = ref mpt.TryGetValue(x);
                ref readonly UserStatus yValue = ref mpt.TryGetValue(y);
                if (xValue.IsNull() & yValue.IsNull()) return x.CompareTo(y);
                if (xValue.IsNull()) return 1;
                if (yValue.IsNull()) return -1;
                ulong xVotes = xValue.SuperPledge + xValue.Votes * 4;
                ulong yVotes = yValue.SuperPledge + yValue.Votes * 4;
                if (xVotes == yVotes) {
                    if (xValue.SuperPledgeTimestamp != yValue.SuperPledgeTimestamp)
                        return xValue.SuperPledgeTimestamp.CompareTo(yValue.SuperPledgeTimestamp);
                    return x.CompareTo(y);
                }
                return yVotes.CompareTo(xVotes);
            }
        }

        unsafe sealed class UserStatusHashAlgorithm : MerklePatriciaTree<Address, UserStatus, Hash<Size256>>.IHashAlgorithm {
            public static readonly MerklePatriciaTree<Address, UserStatus, Hash<Size256>>.IHashAlgorithm Instance = new UserStatusHashAlgorithm();

            private UserStatusHashAlgorithm() { }

            public Hash<Size256> ComputeHash(Address key, in UserStatus value) {
                Span<byte> buffer = stackalloc byte[1024];
                var serializer = new Serializer(buffer);
                serializer.Write(Serializer.U64LittleEndian, value.Balance);
                serializer.Write(Serializer.U64LittleEndian, value.NextNonce);
                serializer.Write(Serializer.U64LittleEndian, value.VotePledge);
                serializer.Write(Serializer.Addresses, value.VoteAddresses);
                serializer.Write(Serializer.U64LittleEndian, value.SuperPledge);
                serializer.Write(Serializer.U64LittleEndian, value.VotePledgeTimestamp);
                serializer.Write(Serializer.U64LittleEndian, value.SuperPledgeTimestamp);
                serializer.Write(Serializer.U64LittleEndian, value.Votes);
                return buffer[..serializer.Index].MessageHash();
            }

            public Hash<Size256> ComputeHash(ReadOnlySpan<Hash<Size256>> hashes) {
                return hashes.HashesHash();
            }
        }

        unsafe sealed class TransactionHashAlgorithm : MerklePatriciaTree<Hash<Size256>, TransactionResult, Hash<Size256>>.IHashAlgorithm {
            public static readonly MerklePatriciaTree<Hash<Size256>, TransactionResult, Hash<Size256>>.IHashAlgorithm Instance = new TransactionHashAlgorithm();

            private TransactionHashAlgorithm() { }

            public Hash<Size256> ComputeHash(Hash<Size256> key, in TransactionResult tx) {
                Span<byte> buffer = stackalloc byte[sizeof(uint)];
                BinaryPrimitives.WriteUInt32LittleEndian(buffer, tx.ErrorCode ?? 0);
                return HashTools.HashesHash(stackalloc[] { key, HashTools.MessageHash(buffer) });
            }

            public Hash<Size256> ComputeHash(ReadOnlySpan<Hash<Size256>> hashes) {
                return hashes.HashesHash();
            }
        }

        sealed class StateMachine : IDisposable {
            private readonly BlockChainSystem system;

            public StateMachine(BlockChainSystem system) => this.system = system;

            private bool isDisposed;

            private void Dispose(bool disposing) {
                if (!isDisposed) {
                    if (disposing) {

                    }
                    isDisposed = true;
                }
            }

            ~StateMachine() {
                Dispose(disposing: false);
            }

            public void Dispose() {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
