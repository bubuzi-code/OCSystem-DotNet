#nullable enable

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace OnlyChain.Core {
    internal static class MerklePatriciaTreeSupport {
        public static readonly NotSupportedException NotSupportedException = new NotSupportedException();

        public interface IBlock { }

        [StructLayout(LayoutKind.Sequential, Size = 1)] public struct Block1 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 2)] public struct Block2 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 3)] public struct Block3 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 4)] public struct Block4 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 5)] public struct Block5 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 6)] public struct Block6 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 7)] public struct Block7 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 8)] public struct Block8 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 9)] public struct Block9 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 10)] public struct Block10 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 11)] public struct Block11 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 12)] public struct Block12 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 13)] public struct Block13 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 14)] public struct Block14 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 15)] public struct Block15 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 16)] public struct Block16 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 17)] public struct Block17 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 18)] public struct Block18 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 19)] public struct Block19 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 20)] public struct Block20 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 21)] public struct Block21 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 22)] public struct Block22 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 23)] public struct Block23 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 24)] public struct Block24 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 25)] public struct Block25 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 26)] public struct Block26 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 27)] public struct Block27 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 28)] public struct Block28 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 29)] public struct Block29 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 30)] public struct Block30 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 31)] public struct Block31 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 32)] public struct Block32 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 33)] public struct Block33 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 34)] public struct Block34 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 35)] public struct Block35 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 36)] public struct Block36 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 37)] public struct Block37 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 38)] public struct Block38 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 39)] public struct Block39 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 40)] public struct Block40 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 41)] public struct Block41 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 42)] public struct Block42 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 43)] public struct Block43 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 44)] public struct Block44 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 45)] public struct Block45 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 46)] public struct Block46 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 47)] public struct Block47 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 48)] public struct Block48 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 49)] public struct Block49 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 50)] public struct Block50 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 51)] public struct Block51 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 52)] public struct Block52 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 53)] public struct Block53 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 54)] public struct Block54 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 55)] public struct Block55 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 56)] public struct Block56 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 57)] public struct Block57 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 58)] public struct Block58 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 59)] public struct Block59 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 60)] public struct Block60 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 61)] public struct Block61 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 62)] public struct Block62 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 63)] public struct Block63 : IBlock { }
        [StructLayout(LayoutKind.Sequential, Size = 64)] public struct Block64 : IBlock { }
    }

    unsafe partial class MerklePatriciaTree<TKey, TValue> {
        internal static class Support {
            delegate INode CreateNodeHandler(byte* key, INode value);

            static readonly CreateNodeHandler[] createNodeHandlers = {
                (key, child) => child,
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block1>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block2>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block3>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block4>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block5>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block6>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block7>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block8>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block9>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block10>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block11>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block12>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block13>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block14>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block15>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block16>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block17>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block18>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block19>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block20>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block21>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block22>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block23>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block24>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block25>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block26>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block27>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block28>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block29>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block30>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block31>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block32>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block33>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block34>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block35>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block36>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block37>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block38>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block39>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block40>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block41>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block42>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block43>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block44>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block45>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block46>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block47>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block48>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block49>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block50>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block51>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block52>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block53>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block54>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block55>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block56>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block57>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block58>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block59>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block60>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block61>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block62>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block63>(key, child),
                (key, child) => new LongPathNode<MerklePatriciaTreeSupport.Block64>(key, child),
            };

            public interface INode {
                bool TryGetValue(byte* key, [MaybeNullWhen(false)] out TValue value);
                INode Add(ref AddArgs args, byte* key, int length);
                IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key);
                INode? Remove(ref RemoveArgs args, byte* key);
                INode PrefixConcat<TBlock>(LongPathNode<TBlock> parent) where TBlock : unmanaged, MerklePatriciaTreeSupport.IBlock => parent;
            }

            public ref struct AddArgs {
                public TValue Value;
                public bool Update;
            }

            public ref struct RemoveArgs {
                public TValue Value;
                public bool NeedCompareValue;
                public bool Result;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static INode CreateLongPathNode(byte* key, int length, INode child) => createNodeHandlers[length](key, child);

            [StructLayout(LayoutKind.Sequential)]
            public sealed class PrefixMapNode : INode {
#pragma warning disable IDE0044
                private INode? child0,
                               child1,
                               child2,
                               child3,
                               child4,
                               child5,
                               child6,
                               child7,
                               child8,
                               child9,
                               child10,
                               child11,
                               child12,
                               child13,
                               child14,
                               child15;
#pragma warning restore IDE0044

                internal ref INode? this[int i] => ref Unsafe.Add(ref child0, i);

                public bool TryGetValue(byte* key, out TValue value) {
                    if (this[*key] is INode child) {
                        return child.TryGetValue(key + 1, out value);
                    }
                    value = default!;
                    return false;
                }

                public INode Add(ref AddArgs args, byte* key, int length) {
                    if (this[*key] is INode child) {
                        this[*key] = child.Add(ref args, key + 1, length - 1);
                    } else {
                        this[*key] = CreateLongPathNode(key + 1, length - 1, new ValueNode(args.Value));
                        args.Update = false;
                    }
                    return this;
                }

                public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key) {
                    for (int i = 0; i < 16; i++) {
                        if (this[i] is INode child) {
                            key[index] = (byte)i;
                            foreach (var kv in child.Enumerate(index + 1, key)) yield return kv;
                        }
                    }
                }

                public INode? Remove(ref RemoveArgs args, byte* key) {
                    if (this[*key] is INode child) {
                        this[*key] = child.Remove(ref args, key + 1);
                        if (!args.Result || this[*key] != null) return this;
                    } else {
                        return this;
                    }

                    byte p1 = 0xff, p2 = 0xff;
                    for (int i = 0; i < 16; i++) {
                        if (this[i] is INode) {
                            p2 = p1;
                            p1 = (byte)i;
                        }
                    }

                    if (p2 != 0xff) return this;

                    var result = new LongPathNode<MerklePatriciaTreeSupport.Block1>(&p1, this[p1]!);
                    return this[p1]!.PrefixConcat(result);
                }
            }

            public sealed class BinaryBranchNode : INode {
                private readonly byte prefix1, prefix2;
                private INode child1, child2;

                public BinaryBranchNode(byte prefix1, byte prefix2, INode child1, INode child2) {
                    this.prefix1 = prefix1;
                    this.prefix2 = prefix2;
                    this.child1 = child1;
                    this.child2 = child2;
                }

                public bool TryGetValue(byte* key, out TValue value) {
                    if (*key == prefix1) return child1.TryGetValue(key + 1, out value);
                    if (*key == prefix2) return child2.TryGetValue(key + 1, out value);
                    value = default!;
                    return false;
                }

                public INode Add(ref AddArgs args, byte* key, int length) {
                    if (*key == prefix1) {
                        child1 = child1.Add(ref args, key + 1, length - 1);
                        return this;
                    }
                    if (*key == prefix2) {
                        child2 = child2.Add(ref args, key + 1, length - 1);
                        return this;
                    }

                    var result = new PrefixMapNode();
                    result[prefix1] = child1;
                    result[prefix2] = child2;
                    result[*key] = CreateLongPathNode(key + 1, length - 1, new ValueNode(args.Value));
                    args.Update = false;
                    return result;
                }

                public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key) {
                    if (prefix1 < prefix2) {
                        key[index] = prefix1;
                        foreach (var kv in child1.Enumerate(index + 1, key)) yield return kv;
                        key[index] = prefix2;
                        foreach (var kv in child2.Enumerate(index + 1, key)) yield return kv;
                    } else {
                        key[index] = prefix2;
                        foreach (var kv in child2.Enumerate(index + 1, key)) yield return kv;
                        key[index] = prefix1;
                        foreach (var kv in child1.Enumerate(index + 1, key)) yield return kv;
                    }
                }

                public INode? Remove(ref RemoveArgs args, byte* key) {
                    byte otherPrefix;
                    ref INode child = ref Unsafe.AsRef<INode>(null);
                    INode otherChild;
                    if (prefix1 == *key) {
                        otherPrefix = prefix2;
                        child = ref child1;
                        otherChild = child2;
                        goto Find;
                    }
                    if (prefix2 == *key) {
                        otherPrefix = prefix1;
                        child = ref child2;
                        otherChild = child1;
                        goto Find;
                    }
                    return this;

                Find:
                    var newChild = child.Remove(ref args, key + 1);
                    if (!args.Result) return this;
                    if (newChild is INode) { child = newChild; return this; }

                    var result = new LongPathNode<MerklePatriciaTreeSupport.Block1>(&otherPrefix, otherChild);
                    return otherChild.PrefixConcat(result);
                }
            }

            public sealed class LongPathNode<TBlock> : INode where TBlock : unmanaged, MerklePatriciaTreeSupport.IBlock {
                static readonly int BlockSize = sizeof(TBlock);

                private TBlock path;
                private INode child;

                public LongPathNode(byte* key, INode child) {
                    new ReadOnlySpan<byte>(key, sizeof(TBlock)).CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.As<TBlock, byte>(ref path), sizeof(TBlock)));
                    this.child = child;
                }

                [MethodImpl(MethodImplOptions.AggressiveOptimization)]
                public INode Add(ref AddArgs args, byte* key, int length) {
                    fixed (TBlock* path = &this.path) {
                        int commonPrefix = 0;
                        for (; commonPrefix < sizeof(TBlock); commonPrefix++) {
                            if (((byte*)path)[commonPrefix] != key[commonPrefix]) goto Split;
                        }

                        child = child.Add(ref args, key + sizeof(TBlock), length - sizeof(TBlock));
                        return this;

                    Split:
                        var binaryNode = new BinaryBranchNode(
                            ((byte*)path)[commonPrefix],
                            key[commonPrefix],
                            CreateLongPathNode((byte*)path + (commonPrefix + 1), sizeof(TBlock) - (commonPrefix + 1), child),
                            CreateLongPathNode(key + (commonPrefix + 1), length - (commonPrefix + 1), new ValueNode(args.Value))
                        );
                        args.Update = false;
                        return CreateLongPathNode(key, commonPrefix, binaryNode);
                    }
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void PathWriteTo(Span<byte> span) {
                    MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<TBlock, byte>(ref path), sizeof(TBlock)).CopyTo(span);
                }

                public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key) {
                    PathWriteTo(key.AsSpan(index));
                    foreach (var kv in child.Enumerate(index + BlockSize, key)) yield return kv;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private bool SequenceEqual(byte* key) {
                    return new ReadOnlySpan<byte>(key, sizeof(TBlock)).SequenceEqual(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<TBlock, byte>(ref path), sizeof(TBlock)));
                }

                public bool TryGetValue(byte* key, out TValue value) {
                    if (SequenceEqual(key)) {
                        return child.TryGetValue(key + sizeof(TBlock), out value);
                    }
                    value = default!;
                    return false;
                }

                public INode? Remove(ref RemoveArgs args, byte* key) {
                    if (SequenceEqual(key)) {
                        if (child.Remove(ref args, key + sizeof(TBlock)) is INode newChild) {
                            if (!args.Result) return this;
                            child = newChild;
                            return child.PrefixConcat(this);
                        }
                        return null;
                    }
                    return this;
                }

                INode INode.PrefixConcat<TBlock2>(LongPathNode<TBlock2> parent) {
                    var path = stackalloc byte[sizeof(TBlock2) + sizeof(TBlock)];
                    parent.PathWriteTo(new Span<byte>(path, sizeof(TBlock2)));
                    PathWriteTo(new Span<byte>(path + sizeof(TBlock2), sizeof(TBlock)));
                    return CreateLongPathNode(path, sizeof(TBlock2) + sizeof(TBlock), child);
                }

                public override string ToString() {
                    var chars = stackalloc char[sizeof(TBlock)];
                    for (int i = 0; i < sizeof(TBlock); i++) {
                        chars[i] = "0123456789abcdef"[Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), i)];
                    }
                    return new string(chars, 0, sizeof(TBlock)) + ", length = " + sizeof(TBlock);
                }
            }

            public sealed class ValueNode : INode {
                private TValue Value;

                public ValueNode(TValue value) => Value = value;

                public INode Add(ref AddArgs args, byte* key, int length) {
                    if (args.Update) {
                        Value = args.Value;
                        return this;
                    } else {
                        throw new ArgumentException();
                    }
                }

                public bool TryGetValue(byte* key, out TValue value) {
                    value = Value;
                    return true;
                }

                public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key) {
                    TKey tempKey = default;
                    fixed (byte* buffer = key) {
                        BufferToKey(buffer, &tempKey);
                    }
                    return new SoleList(tempKey, Value);
                }

                public INode? Remove(ref RemoveArgs args, byte* key) {
                    if (args.NeedCompareValue && !EqualityComparer<TValue>.Default.Equals(Value, args.Value)) {
                        return this;
                    }
                    args.Result = true;
                    return null;
                }

                sealed class SoleList : IReadOnlyList<KeyValuePair<TKey, TValue>> {
                    public readonly KeyValuePair<TKey, TValue> kv;

                    public SoleList(TKey key, TValue value) {
                        kv = new KeyValuePair<TKey, TValue>(key, value);
                    }

                    public KeyValuePair<TKey, TValue> this[int index] {
                        get {
                            if (index != 0) throw new ArgumentOutOfRangeException();
                            return kv;
                        }
                    }

                    public int Count => 1;

                    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this);

                    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

                    sealed class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>> {
                        private readonly SoleList list;
                        private bool got = false;

                        public Enumerator(SoleList list) {
                            this.list = list;
                        }

                        public KeyValuePair<TKey, TValue> Current => list.kv;

                        object? IEnumerator.Current => Current;

                        public void Dispose() {
                            got = false;
                        }

                        public bool MoveNext() {
                            return got ? false : (got = true);
                        }

                        public void Reset() {
                            got = false;
                        }
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static void KeyToBuffer(TKey* key, byte* buffer) {
                for (int i = 0; i < sizeof(TKey); i++) {
                    buffer[2 * i] = (byte)(((byte*)key)[i] >> 4);
                    buffer[2 * i + 1] = (byte)(((byte*)key)[i] & 15);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public static void BufferToKey(byte* buffer, TKey* key) {
                for (int i = 0; i < sizeof(TKey); i++) {
                    ((byte*)key)[i] = (byte)((buffer[2 * i] << 4) | buffer[2 * i + 1]);
                }
            }
        }
    }

    unsafe public partial class MerklePatriciaTree<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> where TKey : unmanaged {
        private Support.INode? root = null;

        public TValue this[TKey key] {
            get {
                if (TryGetValue(key, out var value)) return value;
                throw new KeyNotFoundException($"键值'{key}'不存在");
            }
            set {
                AddOrUpdate(key, value, true);
            }
        }

        public ICollection<TKey> Keys => new KeyEnumerator(this);

        public ICollection<TValue> Values => new ValueEnumerator(this);

        public int Count { get; private set; } = 0;

        public bool IsReadOnly => false;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        private void AddOrUpdate(TKey key, TValue value, bool update) {
            var keyBuffer = stackalloc byte[sizeof(TKey) * 2];
            Support.KeyToBuffer(&key, keyBuffer);
            if (root is null) {
                root = Support.CreateLongPathNode(keyBuffer, sizeof(TKey) * 2, new Support.ValueNode(value));
                Count++;
            } else {
                var args = new Support.AddArgs {
                    Value = value,
                    Update = update,
                };
                root = root.Add(ref args, keyBuffer, sizeof(TKey) * 2);
                if (!args.Update) Count++;
            }
        }

        public void Add(TKey key, TValue value) {
            try {
                AddOrUpdate(key, value, false);
            } catch (ArgumentException) {
                throw new ArgumentException($"键值'{key}'已存在", nameof(key));
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }

        public void Clear() {
            root = null;
            Count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            if (TryGetValue(item.Key, out var value)) {
                return EqualityComparer<TValue>.Default.Equals(value, item.Value);
            }
            return false;
        }

        public bool ContainsKey(TKey key) => TryGetValue(key, out _);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            if (arrayIndex + Count < array.Length) throw new ArgumentOutOfRangeException();
            int i = 0;
            foreach (var kv in this) {
                array[arrayIndex + i++] = kv;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            if (root is null) return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            var buffer = new byte[sizeof(TKey) * 2];
            return root.Enumerate(0, buffer).GetEnumerator();
        }

        public bool Remove(TKey key) {
            if (root is null) return false;

            var keyBuffer = stackalloc byte[sizeof(TKey) * 2];
            Support.KeyToBuffer(&key, keyBuffer);

            var args = new Support.RemoveArgs {
                NeedCompareValue = false,
                Result = false,
            };
            root = root.Remove(ref args, keyBuffer);
            if (args.Result) {
                Count--;
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            if (root is null) return false;

            TKey key = item.Key;
            var keyBuffer = stackalloc byte[sizeof(TKey) * 2];
            Support.KeyToBuffer(&key, keyBuffer);

            var args = new Support.RemoveArgs {
                Value = item.Value,
                NeedCompareValue = true,
                Result = false,
            };
            root = root.Remove(ref args, keyBuffer);
            if (args.Result) {
                Count--;
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
            if (root is null) {
                value = default!;
                return false;
            }

            var keyBuffer = stackalloc byte[sizeof(TKey) * 2];
            Support.KeyToBuffer(&key, keyBuffer);
            return root.TryGetValue(keyBuffer, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        sealed class KeyEnumerator : ICollection<TKey> {
            readonly IDictionary<TKey, TValue> source;

            public KeyEnumerator(IDictionary<TKey, TValue> source) {
                this.source = source;
            }

            public int Count => source.Count;

            public bool IsReadOnly => true;

            public void Add(TKey item) {
                throw MerklePatriciaTreeSupport.NotSupportedException;
            }

            public void Clear() {
                throw MerklePatriciaTreeSupport.NotSupportedException;
            }

            public bool Contains(TKey item) {
                return source.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex) {
                if (arrayIndex + Count < array.Length) throw new ArgumentOutOfRangeException();
                int i = 0;
                foreach (var key in this) {
                    array[arrayIndex + i++] = key;
                }
            }

            public IEnumerator<TKey> GetEnumerator() => source.Select(kv => kv.Key).GetEnumerator();

            public bool Remove(TKey item) {
                throw MerklePatriciaTreeSupport.NotSupportedException;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        sealed class ValueEnumerator : ICollection<TValue> {
            readonly IDictionary<TKey, TValue> source;

            public ValueEnumerator(IDictionary<TKey, TValue> source) {
                this.source = source;
            }

            public int Count => source.Count;

            public bool IsReadOnly => true;

            public void Add(TValue item) {
                throw MerklePatriciaTreeSupport.NotSupportedException;
            }

            public void Clear() {
                throw MerklePatriciaTreeSupport.NotSupportedException;
            }

            public bool Contains(TValue item) => source.Select(kv => kv.Value).Contains(item);

            public void CopyTo(TValue[] array, int arrayIndex) {
                if (arrayIndex + Count < array.Length) throw new ArgumentOutOfRangeException();
                int i = 0;
                foreach (var key in this) {
                    array[arrayIndex + i++] = key;
                }
            }

            public IEnumerator<TValue> GetEnumerator() => source.Select(kv => kv.Value).GetEnumerator();

            public bool Remove(TValue item) {
                throw MerklePatriciaTreeSupport.NotSupportedException;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
