using System;
using System.Runtime.InteropServices;

namespace OnlyChain.Coding {
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ErasureCodingIndex {
        public readonly int DataMissIndex;
        public readonly int ErrorIndex;

        public ErasureCodingIndex(int dataMissIndex, int errorIndex) => (DataMissIndex, ErrorIndex) = (dataMissIndex, errorIndex);

        public void Deconstruct(out int dataMissIndex, out int errorIndex) {
            dataMissIndex = DataMissIndex;
            errorIndex = ErrorIndex;
        }

        public static implicit operator ErasureCodingIndex((int DataMissIndex, int ErrorIndex) pair)
            => new ErasureCodingIndex(pair.DataMissIndex, pair.ErrorIndex);
    }
}
