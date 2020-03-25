using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace OnlyChain.Core {
    public static class Ripemd160 {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("ripemd160", EntryPoint = "compute_hash")]
        unsafe extern static void ComputeHash(void* @in, void* @out);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe public static Hash<Size160> ComputeHash(Hash<Size256> hash) {
            Hash<Size160> result;
            ComputeHash(&hash, &result);
            return result;
        }
    }
}
