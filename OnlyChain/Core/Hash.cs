using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Cryptography;

namespace OnlyChain.Core {
    unsafe public struct Hash<TSize> : IEquatable<Hash<TSize>> where TSize : unmanaged {
        static Hash() {
            if (sizeof(TSize) % 4 != 0) throw new TypeLoadException("Hash长度必须是4的倍数");
        }

        static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        public static readonly Hash<TSize> Empty = new Hash<TSize>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TSize buffer;

        public Hash(ReadOnlySpan<byte> hash) {
            if (hash.Length != sizeof(TSize)) throw new ArgumentException($"必须是{sizeof(TSize)}字节", nameof(hash));

            buffer = Unsafe.As<byte, TSize>(ref MemoryMarshal.GetReference(hash));
        }

        public Hash(ReadOnlySpan<char> hash) {
            if (hash.Length != sizeof(TSize) * 2) throw new ArgumentException($"必须是{sizeof(TSize) * 2}字节", nameof(hash));

            buffer = Hex.Parse<TSize>(hash);
        }

        /// <summary>
        /// 栈上的<see cref="Hash{TSize}"/>对象使用此属性才是安全的。
        /// </summary>
        public Span<byte> Span {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref Unsafe.As<TSize, byte>(ref buffer), sizeof(TSize));
        }

        public static ref Hash<TSize> FromSpan(Span<byte> span) {
            if (span.Length < sizeof(TSize)) throw new ArgumentException($"必须大于等于{sizeof(TSize)}字节", nameof(span));
            return ref Unsafe.As<byte, Hash<TSize>>(ref MemoryMarshal.GetReference(span));
        }

        public static Hash<TSize> Random() {
            Hash<TSize> result = default;
            rng.GetBytes(result.Span);
            return result;
        }

        public override string ToString() => Hex.ToString(this);

        public override int GetHashCode() {
            fixed (Hash<TSize>* p = &this) {
                return ((int*)p)[sizeof(TSize) / 4 - 1];
            }
        }

        public override bool Equals(object obj) => obj is Hash<TSize> other && Equals(other);

        public bool Equals(Hash<TSize> other) {
            fixed (Hash<TSize>* @this = &this) {
                for (int i = 0; i < sizeof(TSize) / 8; i++) {
                    if (((ulong*)@this)[i] != ((ulong*)&other)[i]) return false;
                }
                if (sizeof(TSize) % 8 != 0) {
                    return ((uint*)@this)[sizeof(TSize) / 4 - 1] == ((uint*)&other)[sizeof(TSize) / 4 - 1];
                }
                return true;
            }
        }

        public static bool operator ==(Hash<TSize> left, Hash<TSize> right) => left.Equals(right);
        public static bool operator !=(Hash<TSize> left, Hash<TSize> right) => !(left == right);

        public static implicit operator Hash<TSize>(string strHash) => new Hash<TSize>(strHash);

        public void WriteToBytes(Span<byte> buffer) {
            fixed (Hash<TSize>* p = &this) new ReadOnlySpan<byte>(p, sizeof(TSize)).CopyTo(buffer);
        }

        public byte[] ToArray() {
            var result = new byte[sizeof(TSize)];
            WriteToBytes(result);
            return result;
        }
    }
}
