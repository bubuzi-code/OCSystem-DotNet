using OnlyChain.Core;
using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace OnlyChain.Core {
    public static class Ripemd160 {
        const uint
            LK1 = 0,
            LK2 = 0x5A827999u,
            LK3 = 0x6ED9EBA1u,
            LK4 = 0x8F1BBCDCu,
            LK5 = 0xA953FD4Eu;
        const uint
            RK1 = 0x50A28BE6u,
            RK2 = 0x5C4DD124u,
            RK3 = 0x6D703EF3u,
            RK4 = 0x7A6D76E9u,
            RK5 = 0;

        static readonly int[] Table = {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,   // l offset 1
            11, 14, 15, 12, 5, 8, 7, 9, 11, 13, 14, 15, 6, 7, 9, 8, // l shift  1
            5, 14, 7, 0, 9, 2, 11, 4, 13, 6, 15, 8, 1, 10, 3, 12,   // r offset 1
            8, 9, 9, 11, 13, 15, 15, 5, 7, 7, 8, 11, 14, 14, 12, 6, // r shift  1

            7, 4, 13, 1, 10, 6, 15, 3, 12, 0, 9, 5, 2, 14, 11, 8,   // l offset 2
            7, 6, 8, 13, 11, 9, 7, 15, 7, 12, 15, 9, 11, 7, 13, 12, // l shift  2
            6, 11, 3, 7, 0, 13, 5, 10, 14, 15, 8, 12, 4, 9, 1, 2,   // r offset 2
            9, 13, 15, 7, 12, 8, 9, 11, 7, 7, 12, 7, 6, 15, 13, 11, // r shift  2

            3, 10, 14, 4, 9, 15, 8, 1, 2, 7, 0, 6, 13, 11, 5, 12,   // l offset 3
            11, 13, 6, 7, 14, 9, 13, 15, 14, 8, 13, 6, 5, 12, 7, 5, // l shift  3
            15, 5, 1, 3, 7, 14, 6, 9, 11, 8, 12, 2, 10, 0, 4, 13,   // r offset 3
            9, 7, 15, 11, 8, 6, 6, 14, 12, 13, 5, 14, 13, 13, 7, 5, // r shift  3

            1, 9, 11, 10, 0, 8, 12, 4, 13, 3, 7, 15, 14, 5, 6, 2,   // l offset 4
            11, 12, 14, 15, 14, 15, 9, 8, 9, 14, 5, 6, 8, 6, 5, 12, // l shift  4
            8, 6, 4, 1, 3, 11, 15, 0, 5, 12, 2, 13, 9, 7, 10, 14,   // r offset 4
            15, 5, 8, 11, 14, 14, 6, 14, 6, 9, 12, 9, 12, 5, 15, 8, // r shift  4

            4, 0, 5, 9, 7, 12, 2, 10, 14, 1, 3, 8, 11, 6, 15, 13,   // l offset 5
            9, 15, 5, 11, 6, 8, 13, 12, 5, 12, 13, 14, 11, 8, 5, 6, // l shift  5
            12, 15, 10, 4, 1, 5, 8, 7, 6, 2, 13, 14, 0, 3, 9, 11,   // r offset 5
            8, 5, 12, 9, 12, 5, 14, 6, 8, 13, 6, 5, 15, 13, 11, 11, // r shift  5
        };

        unsafe public static Hash<Size160> ComputeHash(Hash<Size256> hash) {
            static uint F1(uint x, uint y, uint z) => x ^ y ^ z;
            static uint F2(uint x, uint y, uint z) => (x & y) | (~x & z);
            static uint F3(uint x, uint y, uint z) => (x | ~y) ^ z;
            static uint F4(uint x, uint y, uint z) => (x & z) | (y & ~z);
            static uint F5(uint x, uint y, uint z) => x ^ (y | ~z);

            uint* m = stackalloc uint[sizeof(uint) * 16];
            *(Hash<Size256>*)m = hash;
            m[8] = 0x80;
            m[14] = 256;

            uint a1 = 0x67452301u, b1 = 0xEFCDAB89u, c1 = 0x98BADCFEu, d1 = 0x10325476u, e1 = 0xC3D2E1F0u;
            uint a2 = 0x67452301u, b2 = 0xEFCDAB89u, c2 = 0x98BADCFEu, d2 = 0x10325476u, e2 = 0xC3D2E1F0u;

            ref int table = ref MemoryMarshal.GetReference((ReadOnlySpan<int>)Table);

            for (int i = 0; i < 16; i++) {
                var t = BitOperations.RotateLeft(a1 + F1(b1, c1, d1) + m[Unsafe.Add(ref table, i)] + LK1, Unsafe.Add(ref table, i + 16)) + e1;
                a1 = e1;
                e1 = d1;
                d1 = BitOperations.RotateLeft(c1, 10);
                c1 = b1;
                b1 = t;

                System.Diagnostics.Debugger.Break();
                t = BitOperations.RotateLeft(a2 + F5(b2, c2, d2) + m[Unsafe.Add(ref table, i + 32)] + RK1, Unsafe.Add(ref table, i + 48)) + e2;
                a2 = e2;
                e2 = d2;
                d2 = BitOperations.RotateLeft(c2, 10);
                c2 = b2;
                b2 = t;
            }

            table = ref Unsafe.Add(ref table, 64);

            for (int i = 0; i < 16; i++) {
                var t = BitOperations.RotateLeft(a1 + F2(b1, c1, d1) + m[Unsafe.Add(ref table, i)] + LK2, Unsafe.Add(ref table, i + 16)) + e1;
                a1 = e1;
                e1 = d1;
                d1 = BitOperations.RotateLeft(c1, 10);
                c1 = b1;
                b1 = t;

                t = BitOperations.RotateLeft(a2 + F4(b2, c2, d2) + m[Unsafe.Add(ref table, i + 32)] + RK2, Unsafe.Add(ref table, i + 48)) + e2;
                a2 = e2;
                e2 = d2;
                d2 = BitOperations.RotateLeft(c2, 10);
                c2 = b2;
                b2 = t;
            }

            table = ref Unsafe.Add(ref table, 64);

            for (int i = 0; i < 16; i++) {
                var t = BitOperations.RotateLeft(a1 + F3(b1, c1, d1) + m[Unsafe.Add(ref table, i)] + LK3, Unsafe.Add(ref table, i + 16)) + e1;
                a1 = e1;
                e1 = d1;
                d1 = BitOperations.RotateLeft(c1, 10);
                c1 = b1;
                b1 = t;

                t = BitOperations.RotateLeft(a2 + F3(b2, c2, d2) + m[Unsafe.Add(ref table, i + 32)] + RK3, Unsafe.Add(ref table, i + 48)) + e2;
                a2 = e2;
                e2 = d2;
                d2 = BitOperations.RotateLeft(c2, 10);
                c2 = b2;
                b2 = t;
            }

            table = ref Unsafe.Add(ref table, 64);

            for (int i = 0; i < 16; i++) {
                var t = BitOperations.RotateLeft(a1 + F4(b1, c1, d1) + m[Unsafe.Add(ref table, i)] + LK4, Unsafe.Add(ref table, i + 16)) + e1;
                a1 = e1;
                e1 = d1;
                d1 = BitOperations.RotateLeft(c1, 10);
                c1 = b1;
                b1 = t;

                t = BitOperations.RotateLeft(a2 + F2(b2, c2, d2) + m[Unsafe.Add(ref table, i + 32)] + RK4, Unsafe.Add(ref table, i + 48)) + e2;
                a2 = e2;
                e2 = d2;
                d2 = BitOperations.RotateLeft(c2, 10);
                c2 = b2;
                b2 = t;
            }

            table = ref Unsafe.Add(ref table, 64);

            for (int i = 0; i < 16; i++) {
                var t = BitOperations.RotateLeft(a1 + F5(b1, c1, d1) + m[Unsafe.Add(ref table, i)] + LK5, Unsafe.Add(ref table, i + 16)) + e1;
                a1 = e1;
                e1 = d1;
                d1 = BitOperations.RotateLeft(c1, 10);
                c1 = b1;
                b1 = t;

                t = BitOperations.RotateLeft(a2 + F1(b2, c2, d2) + m[Unsafe.Add(ref table, i + 32)] + RK5, Unsafe.Add(ref table, i + 48)) + e2;
                a2 = e2;
                e2 = d2;
                d2 = BitOperations.RotateLeft(c2, 10);
                c2 = b2;
                b2 = t;
            }

            Hash<Size160> result = default;
            Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 0) = 0xEFCDAB89u + c1 + d2;
            Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 1) = 0x98BADCFEu + d1 + e2;
            Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 2) = 0x10325476u + e1 + a2;
            Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 3) = 0xC3D2E1F0u + a1 + b2;
            Unsafe.Add(ref Unsafe.As<Hash<Size160>, uint>(ref result), 4) = 0x67452301u + b1 + c2;
            return result;
        }
    }
}
