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

        public interface ISoleReadOnlyList<T> : IList<T>, IReadOnlyList<T>, IEnumerator<T> {
            bool Got { get; set; }

            int ICollection<T>.Count => 1;
            bool ICollection<T>.IsReadOnly => true;
            void ICollection<T>.Add(T item) => throw NotSupportedException;
            bool ICollection<T>.Remove(T item) => throw NotSupportedException;
            void ICollection<T>.Clear() => throw NotSupportedException;
            bool ICollection<T>.Contains(T item) => EqualityComparer<T>.Default.Equals(Current, item);
            void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
                if ((uint)arrayIndex >= (uint)array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                array[arrayIndex] = Current;
            }

            int IReadOnlyCollection<T>.Count => 1;

            T IReadOnlyList<T>.this[int index] => index == 0 ? Current : throw new ArgumentOutOfRangeException(nameof(index));

            T IList<T>.this[int index] {
                get => index == 0 ? Current : throw new ArgumentOutOfRangeException(nameof(index));
                set => throw NotSupportedException;
            }
            int IList<T>.IndexOf(T item) => EqualityComparer<T>.Default.Equals(Current, item) ? 0 : -1;
            void IList<T>.RemoveAt(int index) => throw NotSupportedException;
            void IList<T>.Insert(int index, T item) => throw NotSupportedException;

            object? IEnumerator.Current => Current;
            IEnumerator IEnumerable.GetEnumerator() => this;
            void IEnumerator.Reset() => throw NotSupportedException;
            bool IEnumerator.MoveNext() => Got ? false : (Got = true);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

            void IDisposable.Dispose() => Got = false;
        }

        public sealed class SoleList<T> : ISoleReadOnlyList<T> {
            bool ISoleReadOnlyList<T>.Got { get; set; } = false;
            public T Current { get; }

            public SoleList(T value) => Current = value;
        }
    }

    unsafe partial class MerklePatriciaTree<TKey, TValue, TState> {
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

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static ValueNode CreateValue(ref AddArgs args) {
                args.Result = true;
                args.Update = false;
                return new ValueNode(args.Value);
            }

            public interface INode {
                bool TryGetValue(byte* key, [MaybeNullWhen(false)] out TValue value);
                INode Add(ref AddArgs args, byte* key, int length);
                IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key);
                INode? Remove(ref RemoveArgs args, byte* key);
                INode PrefixConcat<TBlock>(LongPathNode<TBlock> parent) where TBlock : unmanaged, MerklePatriciaTreeSupport.IBlock => parent;
            }

            public readonly struct SwitchAction {
                private readonly Action<INode, INode?> action;
                private readonly INode target;
                private readonly INode? oldChild, newChild;

                public SwitchAction(Action<INode, INode?> action, INode target, INode? oldChild, INode? newChild) {
                    this.action = action;
                    this.target = target;
                    this.oldChild = oldChild;
                    this.newChild = newChild;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Forward() => action(target, newChild);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Back() => action(target, oldChild);
            }

            public ref struct AddArgs {
                public TValue Value;
                public SwitchAction Switch;
                public bool Result;
                public bool Update;
            }

            public ref struct RemoveArgs {
                public TValue Value;
                public SwitchAction Switch;
                public bool Result;
                public bool NeedCompareValue;
                public bool NeedSetValue;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static INode CreateLongPathNode(byte* key, int length, INode child) => createNodeHandlers[length](key, child);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void SetNewChild(ref SwitchAction @switch, INode node, ref INode? childField, INode? newChild, Action<INode, INode?> switchAction) {
                if (childField == newChild) return;
                @switch = new SwitchAction(switchAction, node, childField, newChild);
                childField = newChild;
            }

            public sealed class EmptyNode : INode {
                private INode? child;

                public INode Add(ref AddArgs args, byte* key, int length) {
                    INode newChild;
                    if (child is null) {
                        newChild = CreateLongPathNode(key, length, CreateValue(ref args));
                    } else {
                        newChild = child.Add(ref args, key, length);
                    }
                    SetNewChild(ref args.Switch, this, ref child, newChild, switchAction);
                    return this;
                }

                public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key) {
                    return child is null ? Enumerable.Empty<KeyValuePair<TKey, TValue>>() : child.Enumerate(index, key);
                }

                public INode? Remove(ref RemoveArgs args, byte* key) {
                    if (child != null) {
                        INode? newChild = child.Remove(ref args, key);
                        SetNewChild(ref args.Switch, this, ref child, newChild, switchAction);
                    }
                    return this;
                }

                public bool TryGetValue(byte* key, out TValue value) {
                    if (child is null) {
                        value = default!;
                        return false;
                    }
                    return child.TryGetValue(key, out value);
                }

                public void Clear() {
                    child = null;
                }

                static readonly Action<INode, INode?> switchAction = (@this, child) => Unsafe.As<EmptyNode>(@this).child = child;
            }

            [StructLayout(LayoutKind.Sequential)]
            public sealed class PrefixMapNode : INode {
                private INode? child_0,
                               child_1,
                               child_2,
                               child_3,
                               child_4,
                               child_5,
                               child_6,
                               child_7,
                               child_8,
                               child_9,
                               child_a,
                               child_b,
                               child_c,
                               child_d,
                               child_e,
                               child_f;

                internal ref INode? this[int i] => ref Unsafe.Add(ref child_0, i);

                public bool TryGetValue(byte* key, out TValue value) {
                    if (this[*key] is INode child) {
                        return child.TryGetValue(key + 1, out value);
                    }
                    value = default!;
                    return false;
                }

                public INode Add(ref AddArgs args, byte* key, int length) {
                    INode? newChild;
                    if (this[*key] is INode child) {
                        newChild = child.Add(ref args, key + 1, length - 1);
                    } else {
                        newChild = CreateLongPathNode(key + 1, length - 1, CreateValue(ref args));
                    }
                    SetNewChild(ref args.Switch, this, ref this[*key], newChild, switchActions[*key]);
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
                        INode? newChild = child.Remove(ref args, key + 1);
                        if (!args.Result) return this;
                        if (newChild != null) {
                            SetNewChild(ref args.Switch, this, ref this[*key], newChild, switchActions[*key]);
                            return this;
                        }
                    } else {
                        return this;
                    }

                    // newChild == null
                    byte p1 = 0xff, p2 = 0xff;
                    for (int i = 0; i < 16; i++) {
                        if (this[i] is INode) {
                            p2 = p1;
                            p1 = (byte)i;
                        }
                    }

                    if (p2 != 0xff) { // 移除后的子节点数量 >= 2
                        SetNewChild(ref args.Switch, this, ref this[*key], null, switchActions[*key]);
                        return this;
                    }

                    var result = new LongPathNode<MerklePatriciaTreeSupport.Block1>(&p1, this[p1]!);
                    return this[p1]!.PrefixConcat(result);
                }

                public override string ToString() {
                    var list = new List<char>(16);
                    for (int i = 0; i < 16; i++) {
                        if (this[i] is INode) list.Add("0123456789abcdef"[i]);
                    }
                    return $"[{string.Join(",", list)}],count={list.Count}";
                }

                static readonly Action<INode, INode?>[] switchActions = {
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_0 = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_1 = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_2 = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_3 = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_4 = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_5 = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_6 = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_7 = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_8 = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_9 = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_a = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_b = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_c = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_d = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_e = child,
                    (@this, child) => Unsafe.As<PrefixMapNode>(@this).child_f = child,
                };
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
                        INode? newChild = child1.Add(ref args, key + 1, length - 1);
                        SetNewChild(ref args.Switch, this, ref child1!, newChild, switchAction1);
                        return this;
                    }
                    if (*key == prefix2) {
                        INode? newChild = child2.Add(ref args, key + 1, length - 1);
                        SetNewChild(ref args.Switch, this, ref child2!, newChild, switchAction2);
                        return this;
                    }

                    var result = new PrefixMapNode();
                    result[prefix1] = child1;
                    result[prefix2] = child2;
                    result[*key] = CreateLongPathNode(key + 1, length - 1, CreateValue(ref args));
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
                    Action<INode, INode?> switchAction;
                    if (prefix1 == *key) {
                        otherPrefix = prefix2;
                        child = ref child1;
                        otherChild = child2;
                        switchAction = switchAction1;
                    } else if (prefix2 == *key) {
                        otherPrefix = prefix1;
                        child = ref child2;
                        otherChild = child1;
                        switchAction = switchAction2;
                    } else {
                        return this;
                    }

                    var newChild = child.Remove(ref args, key + 1);
                    if (!args.Result) return this;
                    if (newChild is INode) {
                        SetNewChild(ref args.Switch, this, ref child!, newChild, switchAction);
                        return this;
                    }

                    var result = new LongPathNode<MerklePatriciaTreeSupport.Block1>(&otherPrefix, otherChild);
                    return otherChild.PrefixConcat(result);
                }

                public override string ToString() {
                    if (prefix1 < prefix2) {
                        return $"[{"0123456789abcdef"[prefix1]},{"0123456789abcdef"[prefix2]}]";
                    } else {
                        return $"[{"0123456789abcdef"[prefix2]},{"0123456789abcdef"[prefix1]}]";
                    }
                }

                static readonly Action<INode, INode?> switchAction1 = (@this, child) => Unsafe.As<BinaryBranchNode>(@this).child1 = child!;
                static readonly Action<INode, INode?> switchAction2 = (@this, child) => Unsafe.As<BinaryBranchNode>(@this).child2 = child!;
            }

            public sealed class LongPathNode<TBlock> : INode where TBlock : unmanaged, MerklePatriciaTreeSupport.IBlock {
                static readonly int BlockSize = sizeof(TBlock);

                private TBlock path;
                private INode child;

                public LongPathNode(byte* key, INode child) {
                    new ReadOnlySpan<byte>(key, sizeof(TBlock)).CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.As<TBlock, byte>(ref path), sizeof(TBlock)));
                    this.child = child;
                }

                public INode Add(ref AddArgs args, byte* key, int length) {
                    fixed (TBlock* path = &this.path) {
                        int commonPrefix = 0;
                        for (; commonPrefix < sizeof(TBlock); commonPrefix++) {
                            if (((byte*)path)[commonPrefix] != key[commonPrefix]) goto Split;
                        }

                        INode? newChild = child.Add(ref args, key + sizeof(TBlock), length - sizeof(TBlock));
                        SetNewChild(ref args.Switch, this, ref child!, newChild, switchAction);
                        return this;

                    Split:
                        var binaryNode = new BinaryBranchNode(
                            ((byte*)path)[commonPrefix],
                            key[commonPrefix],
                            CreateLongPathNode((byte*)path + (commonPrefix + 1), sizeof(TBlock) - (commonPrefix + 1), child),
                            CreateLongPathNode(key + (commonPrefix + 1), length - (commonPrefix + 1), CreateValue(ref args))
                        );
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
                            var result = newChild.PrefixConcat(this);
                            if (result == this) {
                                SetNewChild(ref args.Switch, this, ref child!, newChild, switchAction);
                            }
                            return result;
                        }
                        return null;
                    }
                    return this;
                }

                INode INode.PrefixConcat<TPrefixBlock>(LongPathNode<TPrefixBlock> parent) {
                    var path = stackalloc byte[sizeof(TPrefixBlock) + sizeof(TBlock)];
                    parent.PathWriteTo(new Span<byte>(path, sizeof(TPrefixBlock)));
                    PathWriteTo(new Span<byte>(path + sizeof(TPrefixBlock), sizeof(TBlock)));
                    return CreateLongPathNode(path, sizeof(TPrefixBlock) + sizeof(TBlock), child);
                }

                public override string ToString() {
                    var chars = stackalloc char[sizeof(TBlock)];
                    for (int i = 0; i < sizeof(TBlock); i++) {
                        chars[i] = "0123456789abcdef"[Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), i)];
                    }
                    return new string(chars, 0, sizeof(TBlock)) + ",length=" + sizeof(TBlock);
                }

                static readonly Action<INode, INode?> switchAction = (@this, child) => Unsafe.As<LongPathNode<TBlock>>(@this).child = child!;
            }

            public sealed class ValueNode : INode {
                private readonly TValue value;

                public ValueNode(TValue value) => this.value = value;

                public INode Add(ref AddArgs args, byte* key, int length) {
                    if (args.Update) {
                        args.Result = true;
                        return new ValueNode(args.Value);
                    }
                    return this;
                }

                public bool TryGetValue(byte* key, out TValue value) {
                    value = this.value;
                    return true;
                }

                public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key) {
                    TKey tempKey = default;
                    fixed (byte* buffer = key) {
                        BufferToKey(buffer, &tempKey);
                    }
                    return new MerklePatriciaTreeSupport.SoleList<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(tempKey, value));
                }

                public INode? Remove(ref RemoveArgs args, byte* key) {
                    if (args.NeedCompareValue && !EqualityComparer<TValue>.Default.Equals(value, args.Value)) {
                        return this;
                    }
                    if (args.NeedSetValue) args.Value = value;
                    args.Result = true;
                    return null;
                }

                public override string? ToString() => value?.ToString();
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

        internal sealed class StateMachine {
            interface ISmallDictionary : IDictionary<TState, SwitchInfo> {
                bool ICollection<KeyValuePair<TState, SwitchInfo>>.IsReadOnly => true;
                void ICollection<KeyValuePair<TState, SwitchInfo>>.Add(KeyValuePair<TState, SwitchInfo> item) => throw MerklePatriciaTreeSupport.NotSupportedException;
                void ICollection<KeyValuePair<TState, SwitchInfo>>.Clear() => throw MerklePatriciaTreeSupport.NotSupportedException;
                bool ICollection<KeyValuePair<TState, SwitchInfo>>.Remove(KeyValuePair<TState, SwitchInfo> item) => throw MerklePatriciaTreeSupport.NotSupportedException;

                void IDictionary<TState, SwitchInfo>.Add(TState key, SwitchInfo value) => throw MerklePatriciaTreeSupport.NotSupportedException;
                bool IDictionary<TState, SwitchInfo>.Remove(TState key) => throw MerklePatriciaTreeSupport.NotSupportedException;

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }

            sealed class EmptyDictionary : ISmallDictionary {
                public static readonly IDictionary<TState, SwitchInfo> Default = new EmptyDictionary();

                private EmptyDictionary() { }

                SwitchInfo IDictionary<TState, SwitchInfo>.this[TState key] {
                    get => throw MerklePatriciaTreeSupport.NotSupportedException;
                    set => throw MerklePatriciaTreeSupport.NotSupportedException;
                }

                public ICollection<TState> Keys => Array.Empty<TState>();

                public ICollection<SwitchInfo> Values => Array.Empty<SwitchInfo>();

                public int Count => 0;

                public bool Contains(KeyValuePair<TState, SwitchInfo> item) => false;

                public bool ContainsKey(TState key) => false;

                public void CopyTo(KeyValuePair<TState, SwitchInfo>[] array, int arrayIndex) { }

                public IEnumerator<KeyValuePair<TState, SwitchInfo>> GetEnumerator() => Enumerable.Empty<KeyValuePair<TState, SwitchInfo>>().GetEnumerator();

                public bool TryGetValue(TState key, [MaybeNullWhen(false)] out SwitchInfo value) {
                    value = default!;
                    return false;
                }
            }

            sealed class SoleDictionary : ISmallDictionary, MerklePatriciaTreeSupport.ISoleReadOnlyList<KeyValuePair<TState, SwitchInfo>> {
                private readonly KeyValuePair<TState, SwitchInfo> kv;

                public TState Key => kv.Key;
                public SwitchInfo Value => kv.Value;

                bool MerklePatriciaTreeSupport.ISoleReadOnlyList<KeyValuePair<TState, SwitchInfo>>.Got { get; set; } = false;

                KeyValuePair<TState, SwitchInfo> IEnumerator<KeyValuePair<TState, SwitchInfo>>.Current => kv;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public SoleDictionary(TState key, SwitchInfo value) => kv = new KeyValuePair<TState, SwitchInfo>(key, value);

                SwitchInfo IDictionary<TState, SwitchInfo>.this[TState key] {
                    get => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
                    set => throw MerklePatriciaTreeSupport.NotSupportedException;
                }

                public ICollection<TState> Keys => new MerklePatriciaTreeSupport.SoleList<TState>(kv.Key);

                public ICollection<SwitchInfo> Values => new MerklePatriciaTreeSupport.SoleList<SwitchInfo>(kv.Value);

                public bool IsReadOnly => true;

                object? IEnumerator.Current => ((IEnumerator<KeyValuePair<TState, SwitchInfo>>)this).Current;

                public bool Contains(KeyValuePair<TState, SwitchInfo> item) {
                    if (!TryGetValue(item.Key, out var value)) return false;
                    return item.Value.Equals(value);
                }

                public bool ContainsKey(TState key) => EqualityComparer<TState>.Default.Equals(kv.Key, key);

                public void CopyTo(KeyValuePair<TState, SwitchInfo>[] array, int arrayIndex) {
                    if ((uint)arrayIndex >= (uint)array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                    array[arrayIndex] = kv;
                }

                public bool TryGetValue(TState key, [MaybeNullWhen(false)] out SwitchInfo value) {
                    if (ContainsKey(key)) {
                        value = kv.Value;
                        return true;
                    }
                    value = default!;
                    return false;
                }

                IEnumerator IEnumerable.GetEnumerator()
                    => ((MerklePatriciaTreeSupport.ISoleReadOnlyList<KeyValuePair<TState, SwitchInfo>>)this).GetEnumerator();

                void ICollection<KeyValuePair<TState, SwitchInfo>>.Add(KeyValuePair<TState, SwitchInfo> item)
                    => throw MerklePatriciaTreeSupport.NotSupportedException;

                void ICollection<KeyValuePair<TState, SwitchInfo>>.Clear()
                    => throw MerklePatriciaTreeSupport.NotSupportedException;

                bool ICollection<KeyValuePair<TState, SwitchInfo>>.Remove(KeyValuePair<TState, SwitchInfo> item)
                    => throw MerklePatriciaTreeSupport.NotSupportedException;
            }

            struct SwitchInfo {
                public readonly int IncrementCount;
                public readonly Support.SwitchAction[] SwitchActions;

                public SwitchInfo(int incrementCount, Support.SwitchAction[] switchActions) {
                    IncrementCount = incrementCount;
                    SwitchActions = switchActions;
                }
            }

            private readonly Dictionary<TState, IDictionary<TState, SwitchInfo>> stateTree = new Dictionary<TState, IDictionary<TState, SwitchInfo>>();
            private IDictionary<TState, SwitchInfo> currentNextStatus;

            public List<Support.SwitchAction> SwitchActions { get; } = new List<Support.SwitchAction>();
            public int CurrentCount { get; set; }
            public TState InitState { get; }
            public TState CurrentState { get; private set; }

            public StateMachine(TState initState) {
                InitState = initState;
                CurrentState = initState;
                stateTree.Add(initState, currentNextStatus = EmptyDictionary.Default);
            }

            public void Goto(TState nextState, int newCount) {
                if (EqualityComparer<TState>.Default.Equals(CurrentState, nextState)) return;

                if (currentNextStatus.TryGetValue(nextState, out var switchInfo)) {
                    if (SwitchActions.Count != 0) throw new InvalidOperationException("操作中不允许切换到旧状态");
                    CurrentState = nextState;
                    currentNextStatus = stateTree[nextState];
                    CurrentCount += switchInfo.IncrementCount;
                    Forward(switchInfo.SwitchActions);
                } else if (stateTree.TryGetValue(nextState, out var nextNextStatus)) {
                    if (!nextNextStatus.TryGetValue(CurrentState, out switchInfo)) throw new InvalidOperationException("不允许跨状态切换");
                    if (SwitchActions.Count != 0) throw new InvalidOperationException("操作中不允许切换到旧状态");
                    CurrentState = nextState;
                    currentNextStatus = nextNextStatus;
                    CurrentCount -= switchInfo.IncrementCount;
                    Back(switchInfo.SwitchActions);
                } else {
                    switchInfo = new SwitchInfo(newCount - CurrentCount, SwitchActions.ToArray());
                    SwitchActions.Clear();

                    if (currentNextStatus is EmptyDictionary) {
                        currentNextStatus = new SoleDictionary(nextState, switchInfo);
                        stateTree[CurrentState] = currentNextStatus;
                    } else if (currentNextStatus is SoleDictionary soleDictionary) {
                        currentNextStatus = new Dictionary<TState, SwitchInfo> {
                            [soleDictionary.Key] = soleDictionary.Value,
                            [nextState] = switchInfo
                        };
                        stateTree[CurrentState] = currentNextStatus;
                    } else {
                        currentNextStatus.Add(nextState, switchInfo);
                    }

                    CurrentState = nextState;
                    currentNextStatus = EmptyDictionary.Default;
                    CurrentCount = newCount;
                    stateTree.Add(nextState, currentNextStatus);
                }
            }

            public void Rollback() {
                for (int i = SwitchActions.Count - 1; i >= 0; i--) {
                    SwitchActions[i].Back();
                }
                SwitchActions.Clear();
            }

            private static void Forward(Support.SwitchAction[] switchActions) {
                foreach (var @switch in switchActions) @switch.Forward();
            }

            private static void Back(Support.SwitchAction[] switchActions) {
                for (int i = switchActions.Length - 1; i >= 0; i--) {
                    switchActions[i].Back();
                }
            }
        }
    }

    unsafe public partial class MerklePatriciaTree<TKey, TValue, TState> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> where TKey : unmanaged where TState : notnull {
        private readonly Support.EmptyNode root = new Support.EmptyNode();
        private readonly StateMachine stateMachine;

        public MerklePatriciaTree(TState initState) {
            stateMachine = new StateMachine(initState);
        }

        public void Goto(TState nextState) {
            stateMachine.Goto(nextState, Count);
            Count = stateMachine.CurrentCount;
        }

        public void Rollback() {
            stateMachine.Rollback();
            Count = stateMachine.CurrentCount;
        }

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

        private bool AddOrUpdate(TKey key, TValue value, bool update) {
            var keyBuffer = stackalloc byte[sizeof(TKey) * 2];
            Support.KeyToBuffer(&key, keyBuffer);

            var args = new Support.AddArgs {
                Value = value,
                Update = update,
            };
            root.Add(ref args, keyBuffer, sizeof(TKey) * 2);
            if (args.Result & !args.Update) {
                stateMachine.SwitchActions.Add(args.Switch);
                Count++;
            }
            return args.Result;
        }

        public void Add(TKey key, TValue value) {
            if (!AddOrUpdate(key, value, false)) {
                throw new ArgumentException($"键值'{key}'已存在", nameof(key));
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }

        public bool TryAdd(TKey key, TValue value) => AddOrUpdate(key, value, false);

        public void Clear() {
            root.Clear();
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
            if (Count == 0) yield break;

            var buffer = ArrayRent();
            try {
                foreach (var kv in root.Enumerate(0, buffer)) yield return kv;
            } finally {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            static byte[] ArrayRent() => ArrayPool<byte>.Shared.Rent(sizeof(TKey) * 2);
        }

        public bool Remove(TKey key) {
            if (Count == 0) return false;

            var keyBuffer = stackalloc byte[sizeof(TKey) * 2];
            Support.KeyToBuffer(&key, keyBuffer);

            var args = new Support.RemoveArgs {
            };
            root.Remove(ref args, keyBuffer);
            if (args.Result) {
                stateMachine.SwitchActions.Add(args.Switch);
                Count--;
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            if (Count == 0) return false;

            TKey key = item.Key;
            var keyBuffer = stackalloc byte[sizeof(TKey) * 2];
            Support.KeyToBuffer(&key, keyBuffer);

            var args = new Support.RemoveArgs {
                Value = item.Value,
                NeedCompareValue = true,
            };
            root.Remove(ref args, keyBuffer);
            if (args.Result) {
                stateMachine.SwitchActions.Add(args.Switch);
                Count--;
                return true;
            }
            return false;
        }

        public bool Remove(TKey key, out TValue value) {
            if (Count == 0) {
                value = default!;
                return false;
            }

            var keyBuffer = stackalloc byte[sizeof(TKey) * 2];
            Support.KeyToBuffer(&key, keyBuffer);

            var args = new Support.RemoveArgs {
                NeedSetValue = true,
            };
            root.Remove(ref args, keyBuffer);
            if (args.Result) {
                stateMachine.SwitchActions.Add(args.Switch);
                Count--;
                value = args.Value;
                return true;
            }
            value = default!;
            return false;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
            if (Count == 0) {
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
