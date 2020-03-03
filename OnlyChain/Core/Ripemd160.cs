using OnlyChain.Core;
using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

using static System.Runtime.CompilerServices.MethodImplOptions;

namespace OnlyChain.Core {
    public static class Ripemd160 {
        [MethodImpl(AggressiveInlining)] static uint f1(uint x, uint y, uint z) => x ^ y ^ z;
        [MethodImpl(AggressiveInlining)] static uint f2(uint x, uint y, uint z) => (x & y) | (~x & z);
        [MethodImpl(AggressiveInlining)] static uint f3(uint x, uint y, uint z) => (x | ~y) ^ z;
        [MethodImpl(AggressiveInlining)] static uint f4(uint x, uint y, uint z) => (x & z) | (y & ~z);
        [MethodImpl(AggressiveInlining)] static uint f5(uint x, uint y, uint z) => x ^ (y | ~z);

        [MethodImpl(AggressiveInlining)]
        static void round(ref uint a, uint b, ref uint c, uint d, uint e, uint f, uint x, uint k, int r) {
            a = BitOperations.RotateLeft(a + f + x + k, r) + e;
            c = BitOperations.RotateLeft(c, 10);
        }

        [MethodImpl(AggressiveInlining)] static void L1(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int r) => round(ref a, b, ref c, d, e, f1(b, c, d), x, 0, r);
        [MethodImpl(AggressiveInlining)] static void L2(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int r) => round(ref a, b, ref c, d, e, f2(b, c, d), x, 0x5A827999u, r);
        [MethodImpl(AggressiveInlining)] static void L3(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int r) => round(ref a, b, ref c, d, e, f3(b, c, d), x, 0x6ED9EBA1u, r);
        [MethodImpl(AggressiveInlining)] static void L4(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int r) => round(ref a, b, ref c, d, e, f4(b, c, d), x, 0x8F1BBCDCu, r);
        [MethodImpl(AggressiveInlining)] static void L5(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int r) => round(ref a, b, ref c, d, e, f5(b, c, d), x, 0xA953FD4Eu, r);

        [MethodImpl(AggressiveInlining)] static void R1(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int r) => round(ref a, b, ref c, d, e, f5(b, c, d), x, 0x50A28BE6u, r);
        [MethodImpl(AggressiveInlining)] static void R2(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int r) => round(ref a, b, ref c, d, e, f4(b, c, d), x, 0x5C4DD124u, r);
        [MethodImpl(AggressiveInlining)] static void R3(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int r) => round(ref a, b, ref c, d, e, f3(b, c, d), x, 0x6D703EF3u, r);
        [MethodImpl(AggressiveInlining)] static void R4(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int r) => round(ref a, b, ref c, d, e, f2(b, c, d), x, 0x7A6D76E9u, r);
        [MethodImpl(AggressiveInlining)] static void R5(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int r) => round(ref a, b, ref c, d, e, f1(b, c, d), x, 0, r);

        public static Hash<Size160> ComputeHash(Hash<Size256> hash) {
            uint a1 = 0x67452301u, b1 = 0xEFCDAB89u, c1 = 0x98BADCFEu, d1 = 0x10325476u, e1 = 0xC3D2E1F0u;
            uint a2 = a1, b2 = b1, c2 = c1, d2 = d1, e2 = e1;

            uint m0 = Unsafe.Add(ref Unsafe.As<Hash<Size256>, uint>(ref hash), 0);
            uint m1 = Unsafe.Add(ref Unsafe.As<Hash<Size256>, uint>(ref hash), 1);
            uint m2 = Unsafe.Add(ref Unsafe.As<Hash<Size256>, uint>(ref hash), 2);
            uint m3 = Unsafe.Add(ref Unsafe.As<Hash<Size256>, uint>(ref hash), 3);
            uint m4 = Unsafe.Add(ref Unsafe.As<Hash<Size256>, uint>(ref hash), 4);
            uint m5 = Unsafe.Add(ref Unsafe.As<Hash<Size256>, uint>(ref hash), 5);
            uint m6 = Unsafe.Add(ref Unsafe.As<Hash<Size256>, uint>(ref hash), 6);
            uint m7 = Unsafe.Add(ref Unsafe.As<Hash<Size256>, uint>(ref hash), 7);
            const uint m8 = 0x80, m9 = 0, m10 = 0, m11 = 0, m12 = 0, m13 = 0, m14 = 256, m15 = 0;

            if (!BitConverter.IsLittleEndian) {
                m0 = BinaryPrimitives.ReverseEndianness(m0);
                m1 = BinaryPrimitives.ReverseEndianness(m1);
                m2 = BinaryPrimitives.ReverseEndianness(m2);
                m3 = BinaryPrimitives.ReverseEndianness(m3);
                m4 = BinaryPrimitives.ReverseEndianness(m4);
                m5 = BinaryPrimitives.ReverseEndianness(m5);
                m6 = BinaryPrimitives.ReverseEndianness(m6);
                m7 = BinaryPrimitives.ReverseEndianness(m7);
            }

            L1(ref a1, b1, ref c1, d1, e1, m0, 11);
            R1(ref a2, b2, ref c2, d2, e2, m5, 8);
            L1(ref e1, a1, ref b1, c1, d1, m1, 14);
            R1(ref e2, a2, ref b2, c2, d2, m14, 9);
            L1(ref d1, e1, ref a1, b1, c1, m2, 15);
            R1(ref d2, e2, ref a2, b2, c2, m7, 9);
            L1(ref c1, d1, ref e1, a1, b1, m3, 12);
            R1(ref c2, d2, ref e2, a2, b2, m0, 11);
            L1(ref b1, c1, ref d1, e1, a1, m4, 5);
            R1(ref b2, c2, ref d2, e2, a2, m9, 13);
            L1(ref a1, b1, ref c1, d1, e1, m5, 8);
            R1(ref a2, b2, ref c2, d2, e2, m2, 15);
            L1(ref e1, a1, ref b1, c1, d1, m6, 7);
            R1(ref e2, a2, ref b2, c2, d2, m11, 15);
            L1(ref d1, e1, ref a1, b1, c1, m7, 9);
            R1(ref d2, e2, ref a2, b2, c2, m4, 5);
            L1(ref c1, d1, ref e1, a1, b1, m8, 11);
            R1(ref c2, d2, ref e2, a2, b2, m13, 7);
            L1(ref b1, c1, ref d1, e1, a1, m9, 13);
            R1(ref b2, c2, ref d2, e2, a2, m6, 7);
            L1(ref a1, b1, ref c1, d1, e1, m10, 14);
            R1(ref a2, b2, ref c2, d2, e2, m15, 8);
            L1(ref e1, a1, ref b1, c1, d1, m11, 15);
            R1(ref e2, a2, ref b2, c2, d2, m8, 11);
            L1(ref d1, e1, ref a1, b1, c1, m12, 6);
            R1(ref d2, e2, ref a2, b2, c2, m1, 14);
            L1(ref c1, d1, ref e1, a1, b1, m13, 7);
            R1(ref c2, d2, ref e2, a2, b2, m10, 14);
            L1(ref b1, c1, ref d1, e1, a1, m14, 9);
            R1(ref b2, c2, ref d2, e2, a2, m3, 12);
            L1(ref a1, b1, ref c1, d1, e1, m15, 8);
            R1(ref a2, b2, ref c2, d2, e2, m12, 6);

            L2(ref e1, a1, ref b1, c1, d1, m7, 7);
            R2(ref e2, a2, ref b2, c2, d2, m6, 9);
            L2(ref d1, e1, ref a1, b1, c1, m4, 6);
            R2(ref d2, e2, ref a2, b2, c2, m11, 13);
            L2(ref c1, d1, ref e1, a1, b1, m13, 8);
            R2(ref c2, d2, ref e2, a2, b2, m3, 15);
            L2(ref b1, c1, ref d1, e1, a1, m1, 13);
            R2(ref b2, c2, ref d2, e2, a2, m7, 7);
            L2(ref a1, b1, ref c1, d1, e1, m10, 11);
            R2(ref a2, b2, ref c2, d2, e2, m0, 12);
            L2(ref e1, a1, ref b1, c1, d1, m6, 9);
            R2(ref e2, a2, ref b2, c2, d2, m13, 8);
            L2(ref d1, e1, ref a1, b1, c1, m15, 7);
            R2(ref d2, e2, ref a2, b2, c2, m5, 9);
            L2(ref c1, d1, ref e1, a1, b1, m3, 15);
            R2(ref c2, d2, ref e2, a2, b2, m10, 11);
            L2(ref b1, c1, ref d1, e1, a1, m12, 7);
            R2(ref b2, c2, ref d2, e2, a2, m14, 7);
            L2(ref a1, b1, ref c1, d1, e1, m0, 12);
            R2(ref a2, b2, ref c2, d2, e2, m15, 7);
            L2(ref e1, a1, ref b1, c1, d1, m9, 15);
            R2(ref e2, a2, ref b2, c2, d2, m8, 12);
            L2(ref d1, e1, ref a1, b1, c1, m5, 9);
            R2(ref d2, e2, ref a2, b2, c2, m12, 7);
            L2(ref c1, d1, ref e1, a1, b1, m2, 11);
            R2(ref c2, d2, ref e2, a2, b2, m4, 6);
            L2(ref b1, c1, ref d1, e1, a1, m14, 7);
            R2(ref b2, c2, ref d2, e2, a2, m9, 15);
            L2(ref a1, b1, ref c1, d1, e1, m11, 13);
            R2(ref a2, b2, ref c2, d2, e2, m1, 13);
            L2(ref e1, a1, ref b1, c1, d1, m8, 12);
            R2(ref e2, a2, ref b2, c2, d2, m2, 11);

            L3(ref d1, e1, ref a1, b1, c1, m3, 11);
            R3(ref d2, e2, ref a2, b2, c2, m15, 9);
            L3(ref c1, d1, ref e1, a1, b1, m10, 13);
            R3(ref c2, d2, ref e2, a2, b2, m5, 7);
            L3(ref b1, c1, ref d1, e1, a1, m14, 6);
            R3(ref b2, c2, ref d2, e2, a2, m1, 15);
            L3(ref a1, b1, ref c1, d1, e1, m4, 7);
            R3(ref a2, b2, ref c2, d2, e2, m3, 11);
            L3(ref e1, a1, ref b1, c1, d1, m9, 14);
            R3(ref e2, a2, ref b2, c2, d2, m7, 8);
            L3(ref d1, e1, ref a1, b1, c1, m15, 9);
            R3(ref d2, e2, ref a2, b2, c2, m14, 6);
            L3(ref c1, d1, ref e1, a1, b1, m8, 13);
            R3(ref c2, d2, ref e2, a2, b2, m6, 6);
            L3(ref b1, c1, ref d1, e1, a1, m1, 15);
            R3(ref b2, c2, ref d2, e2, a2, m9, 14);
            L3(ref a1, b1, ref c1, d1, e1, m2, 14);
            R3(ref a2, b2, ref c2, d2, e2, m11, 12);
            L3(ref e1, a1, ref b1, c1, d1, m7, 8);
            R3(ref e2, a2, ref b2, c2, d2, m8, 13);
            L3(ref d1, e1, ref a1, b1, c1, m0, 13);
            R3(ref d2, e2, ref a2, b2, c2, m12, 5);
            L3(ref c1, d1, ref e1, a1, b1, m6, 6);
            R3(ref c2, d2, ref e2, a2, b2, m2, 14);
            L3(ref b1, c1, ref d1, e1, a1, m13, 5);
            R3(ref b2, c2, ref d2, e2, a2, m10, 13);
            L3(ref a1, b1, ref c1, d1, e1, m11, 12);
            R3(ref a2, b2, ref c2, d2, e2, m0, 13);
            L3(ref e1, a1, ref b1, c1, d1, m5, 7);
            R3(ref e2, a2, ref b2, c2, d2, m4, 7);
            L3(ref d1, e1, ref a1, b1, c1, m12, 5);
            R3(ref d2, e2, ref a2, b2, c2, m13, 5);

            L4(ref c1, d1, ref e1, a1, b1, m1, 11);
            R4(ref c2, d2, ref e2, a2, b2, m8, 15);
            L4(ref b1, c1, ref d1, e1, a1, m9, 12);
            R4(ref b2, c2, ref d2, e2, a2, m6, 5);
            L4(ref a1, b1, ref c1, d1, e1, m11, 14);
            R4(ref a2, b2, ref c2, d2, e2, m4, 8);
            L4(ref e1, a1, ref b1, c1, d1, m10, 15);
            R4(ref e2, a2, ref b2, c2, d2, m1, 11);
            L4(ref d1, e1, ref a1, b1, c1, m0, 14);
            R4(ref d2, e2, ref a2, b2, c2, m3, 14);
            L4(ref c1, d1, ref e1, a1, b1, m8, 15);
            R4(ref c2, d2, ref e2, a2, b2, m11, 14);
            L4(ref b1, c1, ref d1, e1, a1, m12, 9);
            R4(ref b2, c2, ref d2, e2, a2, m15, 6);
            L4(ref a1, b1, ref c1, d1, e1, m4, 8);
            R4(ref a2, b2, ref c2, d2, e2, m0, 14);
            L4(ref e1, a1, ref b1, c1, d1, m13, 9);
            R4(ref e2, a2, ref b2, c2, d2, m5, 6);
            L4(ref d1, e1, ref a1, b1, c1, m3, 14);
            R4(ref d2, e2, ref a2, b2, c2, m12, 9);
            L4(ref c1, d1, ref e1, a1, b1, m7, 5);
            R4(ref c2, d2, ref e2, a2, b2, m2, 12);
            L4(ref b1, c1, ref d1, e1, a1, m15, 6);
            R4(ref b2, c2, ref d2, e2, a2, m13, 9);
            L4(ref a1, b1, ref c1, d1, e1, m14, 8);
            R4(ref a2, b2, ref c2, d2, e2, m9, 12);
            L4(ref e1, a1, ref b1, c1, d1, m5, 6);
            R4(ref e2, a2, ref b2, c2, d2, m7, 5);
            L4(ref d1, e1, ref a1, b1, c1, m6, 5);
            R4(ref d2, e2, ref a2, b2, c2, m10, 15);
            L4(ref c1, d1, ref e1, a1, b1, m2, 12);
            R4(ref c2, d2, ref e2, a2, b2, m14, 8);

            L5(ref b1, c1, ref d1, e1, a1, m4, 9);
            R5(ref b2, c2, ref d2, e2, a2, m12, 8);
            L5(ref a1, b1, ref c1, d1, e1, m0, 15);
            R5(ref a2, b2, ref c2, d2, e2, m15, 5);
            L5(ref e1, a1, ref b1, c1, d1, m5, 5);
            R5(ref e2, a2, ref b2, c2, d2, m10, 12);
            L5(ref d1, e1, ref a1, b1, c1, m9, 11);
            R5(ref d2, e2, ref a2, b2, c2, m4, 9);
            L5(ref c1, d1, ref e1, a1, b1, m7, 6);
            R5(ref c2, d2, ref e2, a2, b2, m1, 12);
            L5(ref b1, c1, ref d1, e1, a1, m12, 8);
            R5(ref b2, c2, ref d2, e2, a2, m5, 5);
            L5(ref a1, b1, ref c1, d1, e1, m2, 13);
            R5(ref a2, b2, ref c2, d2, e2, m8, 14);
            L5(ref e1, a1, ref b1, c1, d1, m10, 12);
            R5(ref e2, a2, ref b2, c2, d2, m7, 6);
            L5(ref d1, e1, ref a1, b1, c1, m14, 5);
            R5(ref d2, e2, ref a2, b2, c2, m6, 8);
            L5(ref c1, d1, ref e1, a1, b1, m1, 12);
            R5(ref c2, d2, ref e2, a2, b2, m2, 13);
            L5(ref b1, c1, ref d1, e1, a1, m3, 13);
            R5(ref b2, c2, ref d2, e2, a2, m13, 6);
            L5(ref a1, b1, ref c1, d1, e1, m8, 14);
            R5(ref a2, b2, ref c2, d2, e2, m14, 5);
            L5(ref e1, a1, ref b1, c1, d1, m11, 11);
            R5(ref e2, a2, ref b2, c2, d2, m0, 15);
            L5(ref d1, e1, ref a1, b1, c1, m6, 8);
            R5(ref d2, e2, ref a2, b2, c2, m3, 13);
            L5(ref c1, d1, ref e1, a1, b1, m15, 5);
            R5(ref c2, d2, ref e2, a2, b2, m9, 11);
            L5(ref b1, c1, ref d1, e1, a1, m13, 6);
            R5(ref b2, c2, ref d2, e2, a2, m11, 11);

            Hash<Size160> result = default;
            Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 0) = 0xEFCDAB89u + c1 + d2;
            Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 1) = 0x98BADCFEu + d1 + e2;
            Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 2) = 0x10325476u + e1 + a2;
            Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 3) = 0xC3D2E1F0u + a1 + b2;
            Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 4) = 0x67452301u + b1 + c2;
            if (!BitConverter.IsLittleEndian) {
                Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 0) = BinaryPrimitives.ReverseEndianness(Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 0));
                Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 1) = BinaryPrimitives.ReverseEndianness(Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 1));
                Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 2) = BinaryPrimitives.ReverseEndianness(Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 2));
                Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 3) = BinaryPrimitives.ReverseEndianness(Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 3));
                Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 4) = BinaryPrimitives.ReverseEndianness(Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 4));
            }
            return result;
        }
    }
}
