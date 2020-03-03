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
using System.Text;

namespace OnlyChain.Core {
    unsafe internal static class MerklePatriciaTreeSupport {
        public interface IBlock {
            int Size { get; }
        }

        public interface ISinglePathNode {
            bool SequenceEqual(byte* key);
        }

        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 1)] public struct Block0 : IBlock { public int Size => 0; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 1)] public struct Block1 : IBlock { public int Size => 1; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 1)] public struct Block2 : IBlock { public int Size => 2; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 1)] public struct Block3 : IBlock { public int Size => 3; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 1)] public struct Block4 : IBlock { public int Size => 4; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 1)] public struct Block5 : IBlock { public int Size => 5; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 1)] public struct Block6 : IBlock { public int Size => 6; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 1)] public struct Block7 : IBlock { public int Size => 7; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 1)] public struct Block8 : IBlock { public int Size => 8; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2)] public struct Block9 : IBlock { public int Size => 9; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2)] public struct Block10 : IBlock { public int Size => 10; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2)] public struct Block11 : IBlock { public int Size => 11; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2)] public struct Block12 : IBlock { public int Size => 12; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2)] public struct Block13 : IBlock { public int Size => 13; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2)] public struct Block14 : IBlock { public int Size => 14; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2)] public struct Block15 : IBlock { public int Size => 15; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2)] public struct Block16 : IBlock { public int Size => 16; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 3)] public struct Block17 : IBlock { public int Size => 17; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 3)] public struct Block18 : IBlock { public int Size => 18; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 3)] public struct Block19 : IBlock { public int Size => 19; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 3)] public struct Block20 : IBlock { public int Size => 20; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 3)] public struct Block21 : IBlock { public int Size => 21; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 3)] public struct Block22 : IBlock { public int Size => 22; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 3)] public struct Block23 : IBlock { public int Size => 23; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 3)] public struct Block24 : IBlock { public int Size => 24; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 4)] public struct Block25 : IBlock { public int Size => 25; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 4)] public struct Block26 : IBlock { public int Size => 26; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 4)] public struct Block27 : IBlock { public int Size => 27; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 4)] public struct Block28 : IBlock { public int Size => 28; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 4)] public struct Block29 : IBlock { public int Size => 29; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 4)] public struct Block30 : IBlock { public int Size => 30; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 4)] public struct Block31 : IBlock { public int Size => 31; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 4)] public struct Block32 : IBlock { public int Size => 32; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 5)] public struct Block33 : IBlock { public int Size => 33; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 5)] public struct Block34 : IBlock { public int Size => 34; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 5)] public struct Block35 : IBlock { public int Size => 35; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 5)] public struct Block36 : IBlock { public int Size => 36; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 5)] public struct Block37 : IBlock { public int Size => 37; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 5)] public struct Block38 : IBlock { public int Size => 38; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 5)] public struct Block39 : IBlock { public int Size => 39; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 5)] public struct Block40 : IBlock { public int Size => 40; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 6)] public struct Block41 : IBlock { public int Size => 41; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 6)] public struct Block42 : IBlock { public int Size => 42; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 6)] public struct Block43 : IBlock { public int Size => 43; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 6)] public struct Block44 : IBlock { public int Size => 44; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 6)] public struct Block45 : IBlock { public int Size => 45; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 6)] public struct Block46 : IBlock { public int Size => 46; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 6)] public struct Block47 : IBlock { public int Size => 47; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 6)] public struct Block48 : IBlock { public int Size => 48; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 7)] public struct Block49 : IBlock { public int Size => 49; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 7)] public struct Block50 : IBlock { public int Size => 50; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 7)] public struct Block51 : IBlock { public int Size => 51; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 7)] public struct Block52 : IBlock { public int Size => 52; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 7)] public struct Block53 : IBlock { public int Size => 53; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 7)] public struct Block54 : IBlock { public int Size => 54; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 7)] public struct Block55 : IBlock { public int Size => 55; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 7)] public struct Block56 : IBlock { public int Size => 56; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 8)] public struct Block57 : IBlock { public int Size => 57; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 8)] public struct Block58 : IBlock { public int Size => 58; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 8)] public struct Block59 : IBlock { public int Size => 59; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 8)] public struct Block60 : IBlock { public int Size => 60; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 8)] public struct Block61 : IBlock { public int Size => 61; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 8)] public struct Block62 : IBlock { public int Size => 62; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 8)] public struct Block63 : IBlock { public int Size => 63; }
        [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 8)] public struct Block64 : IBlock { public int Size => 64; }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ParallelBitDeposit64(uint value) {
            if (Bmi2.X64.IsSupported) {
                return BinaryPrimitives.ReverseEndianness(Bmi2.X64.ParallelBitDeposit(BinaryPrimitives.ReverseEndianness(value), 0x0f0f0f0f0f0f0f0fUL));
            }

            ulong r;
            if (BitConverter.IsLittleEndian) {
                // 12 34 56 78             -> 78563412
                // 01 02 03 04 05 06 07 08 -> 0807060504030201
                ((byte*)&r)[0] = (byte)((value >> 4) & 15);
                ((byte*)&r)[1] = (byte)((value >> 0) & 15);
                ((byte*)&r)[2] = (byte)((value >> 12) & 15);
                ((byte*)&r)[3] = (byte)((value >> 8) & 15);
                ((byte*)&r)[4] = (byte)((value >> 20) & 15);
                ((byte*)&r)[5] = (byte)((value >> 16) & 15);
                ((byte*)&r)[6] = (byte)((value >> 28) & 15);
                ((byte*)&r)[7] = (byte)((value >> 24) & 15);
            } else {
                // 12 34 56 78             -> 12345678
                // 01 02 03 04 05 06 07 08 -> 0102030405060708
                ((byte*)&r)[0] = (byte)(((byte*)&value)[0] >> 4);
                ((byte*)&r)[1] = (byte)(((byte*)&value)[0] & 15);
                ((byte*)&r)[2] = (byte)(((byte*)&value)[1] >> 4);
                ((byte*)&r)[3] = (byte)(((byte*)&value)[1] & 15);
                ((byte*)&r)[4] = (byte)(((byte*)&value)[2] >> 4);
                ((byte*)&r)[5] = (byte)(((byte*)&value)[2] & 15);
                ((byte*)&r)[6] = (byte)(((byte*)&value)[3] >> 4);
                ((byte*)&r)[7] = (byte)(((byte*)&value)[3] & 15);
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ParallelBitDeposit32(ushort value) {
            if (Bmi2.IsSupported) {
                return BinaryPrimitives.ReverseEndianness(Bmi2.ParallelBitDeposit(BinaryPrimitives.ReverseEndianness(value), 0x0f0f0f0f));
            }

            uint r;
            if (BitConverter.IsLittleEndian) {
                ((byte*)&r)[0] = (byte)((value >> 4) & 15);
                ((byte*)&r)[1] = (byte)((value >> 0) & 15);
                ((byte*)&r)[2] = (byte)((value >> 12) & 15);
                ((byte*)&r)[3] = (byte)((value >> 8) & 15);
            } else {
                ((byte*)&r)[0] = (byte)(((byte*)&value)[0] >> 4);
                ((byte*)&r)[1] = (byte)(((byte*)&value)[0] & 15);
                ((byte*)&r)[2] = (byte)(((byte*)&value)[1] >> 4);
                ((byte*)&r)[3] = (byte)(((byte*)&value)[1] & 15);
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ParallelBitDeposit16(byte value) {
            if (Bmi2.IsSupported) {
                return BinaryPrimitives.ReverseEndianness((ushort)Bmi2.ParallelBitDeposit(value, 0x0f0f));
            }

            ushort r;
            ((byte*)&r)[0] = (byte)(value >> 4);
            ((byte*)&r)[1] = (byte)(value & 15);
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ParallelBitExtract64(ulong value) {
            if (Bmi2.X64.IsSupported) {
                return BinaryPrimitives.ReverseEndianness((uint)Bmi2.X64.ParallelBitExtract(BinaryPrimitives.ReverseEndianness(value), 0x0f0f0f0f0f0f0f0fUL));
            }

            uint r;
            if (BitConverter.IsLittleEndian) {
                // 01 02 03 04 05 06 07 08 -> 0807060504030201
                // 12 34 56 78 -> 78563412
                value |= value << 12;
                // value = 7867564534231201
                ((byte*)&r)[0] = (byte)(value >> 8);
                ((byte*)&r)[1] = (byte)(value >> 24);
                ((byte*)&r)[2] = (byte)(value >> 40);
                ((byte*)&r)[3] = (byte)(value >> 56);
            } else {
                // 01 02 03 04 05 06 07 08 -> 0102030405060708
                // 12 34 56 78 -> 12345678
                value |= value >> 4;
                // value = 0112233445566778
                ((byte*)&r)[0] = (byte)(value >> 48);
                ((byte*)&r)[1] = (byte)(value >> 32);
                ((byte*)&r)[2] = (byte)(value >> 16);
                ((byte*)&r)[3] = (byte)(value >> 0);
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ParallelBitExtract32(uint value) {
            if (Bmi2.IsSupported) {
                return BinaryPrimitives.ReverseEndianness((ushort)Bmi2.ParallelBitExtract(BinaryPrimitives.ReverseEndianness(value), 0x0f0f0f0f));
            }

            ushort r;
            if (BitConverter.IsLittleEndian) {
                // 01 02 03 04 -> 04030201
                // 12 34       -> 3412
                value |= value << 12;
                // value = 34231201
                ((byte*)&r)[0] = (byte)(value >> 8);
                ((byte*)&r)[1] = (byte)(value >> 24);
            } else {
                // 01 02 03 04 -> 01020304
                // 12 34       -> 1234
                value |= value >> 4;
                // value = 01122334
                ((byte*)&r)[0] = (byte)(value >> 16);
                ((byte*)&r)[1] = (byte)(value >> 0);
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ParallelBitExtract16(ushort value) {
            if (Bmi2.IsSupported) {
                return (byte)Bmi2.ParallelBitExtract(BinaryPrimitives.ReverseEndianness(value), 0x0f0f);
            }
            if (BitConverter.IsLittleEndian) {
                // 01 02 -> 0201
                // 12    -> 12
                return (byte)((value << 4) | (value >> 8));
            } else {
                // 01 02 -> 0102
                // 12    -> 12
                return (byte)(value | (value >> 4));
            }
        }
    }

    unsafe partial class MerklePatriciaTree<TKey, TValue> : IDictionary<TKey, TValue> where TKey : unmanaged {
        unsafe internal static class Support {
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
                INode Add(byte* key, int length, ref AddArgs args);
                IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key);
            }

            public ref struct AddArgs {
                public TValue Value;
                public bool Update;
            }

            public static INode CreateLongPathNode(byte* key, int length, INode child) => createNodeHandlers[length](key, child);

            public sealed class PrefixMapNode : INode {
                private readonly INode?[] children = new INode?[16];

                public PrefixMapNode(INode?[] children) => this.children = children;

                public bool TryGetValue(byte* key, out TValue value) {
                    if (children[*key] is INode child) {
                        return child.TryGetValue(key + 1, out value);
                    }
                    value = default!;
                    return false;
                }

                public INode Add(byte* key, int length, ref AddArgs args) {
                    if (children[*key] is INode child) {
                        children[*key] = child.Add(key + 1, length - 1, ref args);
                    } else {
                        children[*key] = CreateLongPathNode(key + 1, length - 1, new ValueNode(args.Value));
                        args.Update = false;
                    }
                    return this;
                }

                public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key) {
                    for (int i = 0; i < children.Length; i++) {
                        if (children[i] is INode child) {
                            key[index] = (byte)i;
                            foreach (var kv in child.Enumerate(index + 1, key)) yield return kv;
                        }
                    }
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

                public INode Add(byte* key, int length, ref AddArgs args) {
                    if (*key == prefix1) {
                        child1 = child1.Add(key + 1, length - 1, ref args);
                        return this;
                    }
                    if (*key == prefix2) {
                        child2 = child2.Add(key + 1, length - 1, ref args);
                        return this;
                    }
                    var children = new INode?[16];
                    children[prefix1] = child1;
                    children[prefix2] = child2;
                    children[*key] = CreateLongPathNode(key + 1, length - 1, new ValueNode(args.Value));
                    args.Update = false;
                    return new PrefixMapNode(children);
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
            }

            public sealed class LongPathNode<TBlock> : INode where TBlock : unmanaged, MerklePatriciaTreeSupport.IBlock {
                private TBlock path;
                private INode child;

                public LongPathNode(byte* key, INode child) {
                    Assign(key);
                    this.child = child;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                bool AlignSequenceEqual1(int index, byte* key)
                    => Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index) >> 4 == key[index * 2];

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                bool AlignSequenceEqual2(int index, byte* key)
                    => MerklePatriciaTreeSupport.ParallelBitDeposit16(Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index)) == *(ushort*)(key + index * 2);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                bool AlignSequenceEqual4(int index, byte* key)
                    => MerklePatriciaTreeSupport.ParallelBitDeposit32(Unsafe.As<byte, ushort>(ref Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index))) == *(uint*)(key + index * 2);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                bool AlignSequenceEqual8(int index, byte* key)
                    => MerklePatriciaTreeSupport.ParallelBitDeposit64(Unsafe.As<byte, uint>(ref Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index))) == *(ulong*)(key + index * 2);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void AlignAssign1(int index, byte* key)
                    => Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index) = (byte)(key[index * 2] << 4);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void AlignAssign2(int index, byte* key)
                    => Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index) = MerklePatriciaTreeSupport.ParallelBitExtract16(*(ushort*)(key + index * 2));

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void AlignAssign4(int index, byte* key)
                    => Unsafe.As<byte, ushort>(ref Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index)) = MerklePatriciaTreeSupport.ParallelBitExtract32(*(uint*)(key + index * 2));

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void AlignAssign8(int index, byte* key)
                    => Unsafe.As<byte, uint>(ref Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index)) = MerklePatriciaTreeSupport.ParallelBitExtract64(*(ulong*)(key + index * 2));

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void AlignFill1(int index, byte* buffer)
                    => buffer[index * 2] = (byte)(Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index) >> 4);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void AlignFill2(int index, byte* buffer)
                    => *(ushort*)(buffer + index * 2) = MerklePatriciaTreeSupport.ParallelBitDeposit16(Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index));

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void AlignFill4(int index, byte* buffer)
                    => *(uint*)(buffer + index * 2) = MerklePatriciaTreeSupport.ParallelBitDeposit32(Unsafe.As<byte, ushort>(ref Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index)));

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void AlignFill8(int index, byte* buffer)
                    => *(ulong*)(buffer + index * 2) = MerklePatriciaTreeSupport.ParallelBitDeposit64(Unsafe.As<byte, uint>(ref Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), index)));


                [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                bool SingleSequenceEqual<TIndex>(byte* key) where TIndex : unmanaged, MerklePatriciaTreeSupport.IBlock {
                    if (default(TIndex).Size * 2 >= path.Size) return true;
                    int size = path.Size - default(TIndex).Size * 2;
                    if (size == 1) return AlignSequenceEqual1(default(TIndex).Size, key);
                    if (size == 2) return AlignSequenceEqual2(default(TIndex).Size, key);
                    if (size == 3) return AlignSequenceEqual2(default(TIndex).Size, key) && AlignSequenceEqual1(default(TIndex).Size + 1, key);
                    if (size == 4) return AlignSequenceEqual4(default(TIndex).Size, key);
                    if (size == 5) return AlignSequenceEqual4(default(TIndex).Size, key) && AlignSequenceEqual1(default(TIndex).Size + 2, key);
                    if (size == 6) return AlignSequenceEqual4(default(TIndex).Size, key) && AlignSequenceEqual2(default(TIndex).Size + 2, key);
                    if (size == 7) return AlignSequenceEqual4(default(TIndex).Size, key) && AlignSequenceEqual2(default(TIndex).Size + 2, key) && AlignSequenceEqual1(default(TIndex).Size + 3, key);
                    return AlignSequenceEqual8(default(TIndex).Size, key);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                void SingleAssign<TIndex>(byte* key) where TIndex : unmanaged, MerklePatriciaTreeSupport.IBlock {
                    if (default(TIndex).Size * 2 >= path.Size) return;
                    int size = path.Size - default(TIndex).Size * 2;
                    if (size == 1) {
                        AlignAssign1(default(TIndex).Size, key);
                    } else if (size == 2) {
                        AlignAssign2(default(TIndex).Size, key);
                    } else if (size == 3) {
                        AlignAssign2(default(TIndex).Size, key); AlignAssign1(default(TIndex).Size + 1, key);
                    } else if (size == 4) {
                        AlignAssign4(default(TIndex).Size, key);
                    } else if (size == 5) {
                        AlignAssign4(default(TIndex).Size, key); AlignAssign1(default(TIndex).Size + 2, key);
                    } else if (size == 6) {
                        AlignAssign4(default(TIndex).Size, key); AlignAssign2(default(TIndex).Size + 2, key);
                    } else if (size == 7) {
                        AlignAssign4(default(TIndex).Size, key); AlignAssign2(default(TIndex).Size + 2, key); AlignAssign1(default(TIndex).Size + 3, key);
                    } else {
                        AlignAssign8(default(TIndex).Size, key);
                    }
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                void SingleFill<TIndex>(byte* buffer) where TIndex : unmanaged, MerklePatriciaTreeSupport.IBlock {
                    if (default(TIndex).Size * 2 >= path.Size) return;
                    int size = path.Size - default(TIndex).Size * 2;
                    if (size == 1) {
                        AlignFill1(default(TIndex).Size, buffer);
                    } else if (size == 2) {
                        AlignFill2(default(TIndex).Size, buffer);
                    } else if (size == 3) {
                        AlignFill2(default(TIndex).Size, buffer); AlignFill1(default(TIndex).Size + 1, buffer);
                    } else if (size == 4) {
                        AlignFill4(default(TIndex).Size, buffer);
                    } else if (size == 5) {
                        AlignFill4(default(TIndex).Size, buffer); AlignFill1(default(TIndex).Size + 2, buffer);
                    } else if (size == 6) {
                        AlignFill4(default(TIndex).Size, buffer); AlignFill2(default(TIndex).Size + 2, buffer);
                    } else if (size == 7) {
                        AlignFill4(default(TIndex).Size, buffer); AlignFill2(default(TIndex).Size + 2, buffer); AlignFill1(default(TIndex).Size + 3, buffer);
                    } else {
                        AlignFill8(default(TIndex).Size, buffer);
                    }
                }

                bool SequenceEqual(byte* key) {
                    return SingleSequenceEqual<MerklePatriciaTreeSupport.Block0>(key)
                        && SingleSequenceEqual<MerklePatriciaTreeSupport.Block4>(key)
                        && SingleSequenceEqual<MerklePatriciaTreeSupport.Block8>(key)
                        && SingleSequenceEqual<MerklePatriciaTreeSupport.Block12>(key)
                        && SingleSequenceEqual<MerklePatriciaTreeSupport.Block16>(key)
                        && SingleSequenceEqual<MerklePatriciaTreeSupport.Block20>(key)
                        && SingleSequenceEqual<MerklePatriciaTreeSupport.Block24>(key)
                        && SingleSequenceEqual<MerklePatriciaTreeSupport.Block28>(key);
                }

                void Assign(byte* key) {
                    SingleAssign<MerklePatriciaTreeSupport.Block0>(key);
                    SingleAssign<MerklePatriciaTreeSupport.Block4>(key);
                    SingleAssign<MerklePatriciaTreeSupport.Block8>(key);
                    SingleAssign<MerklePatriciaTreeSupport.Block12>(key);
                    SingleAssign<MerklePatriciaTreeSupport.Block16>(key);
                    SingleAssign<MerklePatriciaTreeSupport.Block20>(key);
                    SingleAssign<MerklePatriciaTreeSupport.Block24>(key);
                    SingleAssign<MerklePatriciaTreeSupport.Block28>(key);
                }

                void FillKey(byte* buffer) {
                    SingleFill<MerklePatriciaTreeSupport.Block0>(buffer);
                    SingleFill<MerklePatriciaTreeSupport.Block4>(buffer);
                    SingleFill<MerklePatriciaTreeSupport.Block8>(buffer);
                    SingleFill<MerklePatriciaTreeSupport.Block12>(buffer);
                    SingleFill<MerklePatriciaTreeSupport.Block16>(buffer);
                    SingleFill<MerklePatriciaTreeSupport.Block20>(buffer);
                    SingleFill<MerklePatriciaTreeSupport.Block24>(buffer);
                    SingleFill<MerklePatriciaTreeSupport.Block28>(buffer);
                }

                void FillKey(byte[] buffer, int index) {
                    fixed (byte* p = &buffer[index]) FillKey(p);
                }

                public bool TryGetValue(byte* key, out TValue value) {
                    if (SequenceEqual(key)) {
                        return child.TryGetValue(key + path.Size, out value);
                    }
                    value = default!;
                    return false;
                }

                public INode Add(byte* key, int length, ref AddArgs args) {
                    int commonPrefix = 0;
                    for (; commonPrefix < path.Size; commonPrefix++) {
                        byte b = Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), commonPrefix / 2);
                        if ((commonPrefix & 1) == 0) {
                            b >>= 4;
                        } else {
                            b &= 15;
                        }
                        if (b != key[commonPrefix]) goto Split;
                    }
                    if ((path.Size & 1) != 0) {
                        if ((Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), path.Size / 2) >> 4) != key[path.Size - 1]) goto Split;
                    }

                    child = child.Add(key + path.Size, length - path.Size, ref args);
                    return this;

                Split:
                    byte* thisKey = stackalloc byte[path.Size];
                    FillKey(thisKey);
                    var binaryNode = new BinaryBranchNode(
                        (commonPrefix & 1) == 0 ? (byte)(Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), commonPrefix / 2) >> 4) : (byte)(Unsafe.Add(ref Unsafe.As<TBlock, byte>(ref path), commonPrefix / 2) & 15),
                        key[commonPrefix],
                        CreateLongPathNode(thisKey + (commonPrefix + 1), path.Size - (commonPrefix + 1), child),
                        CreateLongPathNode(key + (commonPrefix + 1), length - (commonPrefix + 1), new ValueNode(args.Value))
                    );
                    args.Update = false;
                    return CreateLongPathNode(key, commonPrefix, binaryNode);
                }

                public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate(int index, byte[] key) {
                    FillKey(key, index);
                    foreach (var kv in child.Enumerate(index + path.Size, key)) yield return kv;
                }
            }

            public sealed class ValueNode : INode {
                private TValue Value;

                public ValueNode(TValue value) => Value = value;

                public INode Add(byte* key, int length, ref AddArgs args) {
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
                    TKey tempKey;
                    fixed (byte* buffer = key) {
                        MerklePatriciaTreeKeySupport<TKey>.GetKey(buffer, (byte*)&tempKey);
                    }
                    return new SoleList(tempKey, Value);
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

                        public KeyValuePair<TKey, TValue> Current => got ? list.kv : default;

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
        }
    }

    unsafe internal static class MerklePatriciaTreeKeySupport<TKey> where TKey : unmanaged {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AlignFill2(int index, byte* buffer, TKey key)
            => *(ushort*)(buffer + index * 2) = MerklePatriciaTreeSupport.ParallelBitDeposit16(Unsafe.Add(ref Unsafe.As<TKey, byte>(ref key), index));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AlignFill4(int index, byte* buffer, TKey key)
            => *(uint*)(buffer + index * 2) = MerklePatriciaTreeSupport.ParallelBitDeposit32(Unsafe.As<byte, ushort>(ref Unsafe.Add(ref Unsafe.As<TKey, byte>(ref key), index)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AlignFill8(int index, byte* buffer, TKey key)
            => *(ulong*)(buffer + index * 2) = MerklePatriciaTreeSupport.ParallelBitDeposit64(Unsafe.As<byte, uint>(ref Unsafe.Add(ref Unsafe.As<TKey, byte>(ref key), index)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AlignGet2(int index, byte* buffer, byte* key)
          => key[index] = MerklePatriciaTreeSupport.ParallelBitExtract16(*(ushort*)(buffer + index * 2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AlignGet4(int index, byte* buffer, byte* key)
            => *(ushort*)(key + index) = MerklePatriciaTreeSupport.ParallelBitExtract32(*(uint*)(buffer + index * 2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AlignGet8(int index, byte* buffer, byte* key)
            => *(uint*)(key + index) = MerklePatriciaTreeSupport.ParallelBitExtract64(*(ulong*)(buffer + index * 2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SingleFill(int index, byte* buffer, TKey key) {
            if (index >= sizeof(TKey)) return;
            int size = sizeof(TKey) - index;
            if (size == 1) {
                AlignFill2(index, buffer, key);
            } else if (size == 2) {
                AlignFill4(index, buffer, key);
            } else if (size == 3) {
                AlignFill4(index, buffer, key); AlignFill2(index + 2, buffer, key);
            } else {
                AlignFill8(index, buffer, key);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SingleGet(int index, byte* buffer, byte* key) {
            if (index >= sizeof(TKey)) return;
            int size = sizeof(TKey) - index;
            if (size == 1) {
                AlignGet2(index, buffer, key);
            } else if (size == 2) {
                AlignGet4(index, buffer, key);
            } else if (size == 3) {
                AlignGet4(index, buffer, key); AlignGet2(index + 2, buffer, key);
            } else {
                AlignGet8(index, buffer, key);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FillKey(byte* buffer, TKey key) {
            SingleFill(0, buffer, key);
            SingleFill(4, buffer, key);
            SingleFill(8, buffer, key);
            SingleFill(12, buffer, key);
            SingleFill(16, buffer, key);
            SingleFill(20, buffer, key);
            SingleFill(24, buffer, key);
            SingleFill(28, buffer, key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetKey(byte* buffer, byte* key) {
            SingleGet(0, buffer, key);
            SingleGet(4, buffer, key);
            SingleGet(8, buffer, key);
            SingleGet(12, buffer, key);
            SingleGet(16, buffer, key);
            SingleGet(20, buffer, key);
            SingleGet(24, buffer, key);
            SingleGet(28, buffer, key);
        }
    }

    unsafe public partial class MerklePatriciaTree<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> where TKey : unmanaged {
        static MerklePatriciaTree() {
            if (sizeof(TKey) > 32) throw new TypeLoadException($"{typeof(TKey)}不能作为Key，因为超过了32字节");
        }

        private Support.INode? root = null;
        private int count = 0;

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

        public int Count => count;

        public bool IsReadOnly => false;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public void Add(TKey key, TValue value) {
            try {
                AddOrUpdate(key, value, false);
            } catch (ArgumentException) {
                throw new ArgumentException($"键值'{key}'已存在", nameof(key));
            }
        }

        private void AddOrUpdate(TKey key, TValue value, bool update) {
            var keyBuffer = stackalloc byte[sizeof(TKey) * 2];
            MerklePatriciaTreeKeySupport<TKey>.FillKey(keyBuffer, key);
            if (root is null) {
                root = Support.CreateLongPathNode(keyBuffer, sizeof(TKey) * 2, new Support.ValueNode(value));
                count++;
            } else {
                var args = new Support.AddArgs {
                    Value = value,
                    Update = update,
                };
                root = root.Add(keyBuffer, sizeof(TKey) * 2, ref args);
                if (!args.Update) count++;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            if (TryGetValue(item.Key, out var value)) {
                return EqualityComparer<TValue>.Default.Equals(value, item.Value);
            }
            return false;
        }

        public bool ContainsKey(TKey key) => TryGetValue(key, out _);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            if (arrayIndex + count < array.Length) throw new ArgumentOutOfRangeException();
            int i = 0;
            foreach (var kv in this) {
                array[arrayIndex + i++] = kv;
            }
        }

        public bool Remove(TKey key) {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
            if (root is null) {
                value = default!;
                return false;
            }

            var keyBuffer = stackalloc byte[sizeof(TKey) * 2];
            MerklePatriciaTreeKeySupport<TKey>.FillKey(keyBuffer, key);
            return root.TryGetValue(keyBuffer, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            if (root is null) return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            var buffer = new byte[sizeof(TKey) * 2];
            return root.Enumerate(0, buffer).GetEnumerator();
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
                throw new NotImplementedException();
            }

            public void Clear() {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            public void Clear() {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
