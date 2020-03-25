using System;
using System.Runtime.InteropServices;
using System.Security;

namespace OnlyChain.Coding {
    unsafe public static class ErasureCoding {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("erasure_coding", EntryPoint = "ec_encode")]
        extern static int NativeEncode(byte* data, int dataBytes, byte* erasureCode, int erasureCodeBytes, int dataStride, int erasureCodeStride);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("erasure_coding", EntryPoint = "ec_decode")]
        extern static int NativeDecode(byte* dataWithEC, int dataWithECBytes, int stride, ErasureCodingIndex* indexes, int indexCount);

        public static void Encode(ReadOnlySpan<byte> data, Span<byte> erasureCode, int dataStride, int erasureCodeStride) {
            fixed (byte* pData = data)
            fixed (byte* pEC = erasureCode) {
                int errorCode = NativeEncode(pData, data.Length, pEC, erasureCode.Length, dataStride, erasureCodeStride);
                if (errorCode == 0) return;
                switch (errorCode) {
                    case 1: throw new ArgumentOutOfRangeException(nameof(dataStride));
                    case 2: throw new ArgumentOutOfRangeException(nameof(erasureCodeStride));
                    case 3: throw new ArgumentException($"{nameof(data)}的字节数必须是{nameof(dataStride)}的整数倍");
                    case 4: throw new ArgumentException($"{nameof(erasureCode)}的字节数必须是{nameof(erasureCodeStride)}的整数倍");
                    case 5: throw new ArgumentException($"{nameof(data)}的行数必须和{nameof(erasureCode)}一致");
                }
            }
        }

        public static void Decode(Span<byte> dataWithEC, int stride, ErasureCodingIndex[] indexes) {
            fixed (byte* pDataWithEC = dataWithEC)
            fixed (ErasureCodingIndex* pIndexes = indexes) {
                int errorCode = NativeDecode(pDataWithEC, dataWithEC.Length, stride, pIndexes, indexes.Length);
                if (errorCode == 0) return;
                switch (errorCode) {
                    case 1: throw new ArgumentOutOfRangeException(nameof(stride));
                    case 2: throw new ArgumentOutOfRangeException(nameof(indexes), $"{nameof(indexes)}数量过多");
                    case 3: throw new ArgumentException($"{nameof(dataWithEC)}的字节数必须是{nameof(stride)}的整数倍");
                    case 4: throw new ArgumentException(nameof(indexes), $"{nameof(indexes)}包含无效的索引");
                }
            }
        }
    }
}
