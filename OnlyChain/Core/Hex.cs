using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OnlyChain.Core {
    public static class Hex {
        static readonly byte[] CharToHexTable = {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 15
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 31
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 47
            0x0,  0x1,  0x2,  0x3,  0x4,  0x5,  0x6,  0x7,  0x8,  0x9,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 63
            0xFF, 0xA,  0xB,  0xC,  0xD,  0xE,  0xF,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 79
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 95
            0xFF, 0xa,  0xb,  0xc,  0xd,  0xe,  0xf // 102
        };

        unsafe public static T Parse<T>(ReadOnlySpan<char> hexChars) where T : unmanaged {
            if (hexChars.Length != sizeof(T) * 2) throw new ArgumentOutOfRangeException(nameof(hexChars), $"必须是{sizeof(T) * 2}个字符");

            ref byte table = ref MemoryMarshal.GetReference((ReadOnlySpan<byte>)CharToHexTable);
            ref char charFirst = ref MemoryMarshal.GetReference(hexChars);
            T r = default;
            for (int i = 0; i < sizeof(T); i++) {
                if (Unsafe.Add(ref charFirst, 2 * i) > 'f' || Unsafe.Add(ref charFirst, 2 * i + 1) > 'f') throw new ArgumentException("无效的十六进制字串", nameof(hexChars));
                byte v1 = Unsafe.Add(ref table, Unsafe.Add(ref charFirst, 2 * i));
                byte v2 = Unsafe.Add(ref table, Unsafe.Add(ref charFirst, 2 * i + 1));
                if (v1 == 0xff || v2 == 0xff) throw new ArgumentException("无效的十六进制字串", nameof(hexChars));

                ((byte*)&r)[i] = (byte)((v1 << 4) | v2);
            }
            return r;
        }

        static readonly char[] HexTemplate = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        unsafe public static string ToString(ReadOnlySpan<byte> bytes) {
            ref char hexTemplate = ref MemoryMarshal.GetReference((ReadOnlySpan<char>)HexTemplate);
            ref byte buffer = ref MemoryMarshal.GetReference(bytes);
            char* chars = stackalloc char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++) {
                var v = Unsafe.Add(ref buffer, i);
                chars[2 * i] = Unsafe.Add(ref hexTemplate, v >> 4);
                chars[2 * i + 1] = Unsafe.Add(ref hexTemplate, v & 15);
            }
            return new string(chars, 0, bytes.Length * 2);
        }

        unsafe public static string ToString<T>(T bytes) where T : unmanaged {
            ref char hexTemplate = ref MemoryMarshal.GetReference((ReadOnlySpan<char>)HexTemplate);
            char* chars = stackalloc char[sizeof(T) * 2];
            for (int i = 0; i < sizeof(T); i++) {
                byte v = ((byte*)&bytes)[i];
                chars[2 * i] = Unsafe.Add(ref hexTemplate, v >> 4);
                chars[2 * i + 1] = Unsafe.Add(ref hexTemplate, v & 15);
            }
            return new string(chars, 0, sizeof(T) * 2);
        }
    }
}
