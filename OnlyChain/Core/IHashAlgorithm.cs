using System;
using System.Collections.Generic;
using System.Text;

namespace OnlyChain.Core {
    public interface IHashAlgorithm<T, THash> where T : notnull where THash : unmanaged {
        THash ComputeHash(T value);
        THash ComputeHash(ReadOnlySpan<THash> hashes);
    }
}
