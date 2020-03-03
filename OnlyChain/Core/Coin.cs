using System;
using System.Collections.Generic;
using System.Text;

namespace OnlyChain.Core {
    /// <summary>
    /// Only币，精度为亿分之一
    /// </summary>
    public readonly struct Coin {
        const ulong TotalCoin = 1_000_000_000_000_000_000;

        private readonly ulong value;

        public Coin(ulong value) => this.value = value <= TotalCoin ? value : throw new ArgumentOutOfRangeException(nameof(value), "超出系统最大代币量");
        public Coin(long value) => this.value = value >= 0 && value <= (long)TotalCoin ? (ulong)value : throw new ArgumentOutOfRangeException(nameof(value), "超出范围");

        public static implicit operator ulong(Coin @this) => @this.value;
        public static implicit operator Coin(ulong value) => new Coin(value);
        public static implicit operator long(Coin @this) => (long)@this.value;
        public static implicit operator Coin(long value) => new Coin(value);

        public static Coin FromWhole(decimal value) => value >= 0 ? new Coin((ulong)(value * 10000_0000m)) : throw new ArgumentOutOfRangeException(nameof(value), "代币数量不能小于0");

        unsafe public override string ToString() {
            char* buffer = stackalloc char[21]; // 最大20位数字+1位小数点
            var value = this.value;

            buffer[20] = (char)(value % 10 + '0'); value /= 10;
            buffer[19] = (char)(value % 10 + '0'); value /= 10;
            buffer[18] = (char)(value % 10 + '0'); value /= 10;
            buffer[17] = (char)(value % 10 + '0'); value /= 10;
            buffer[16] = (char)(value % 10 + '0'); value /= 10;
            buffer[15] = (char)(value % 10 + '0'); value /= 10;
            buffer[14] = (char)(value % 10 + '0'); value /= 10;
            buffer[13] = (char)(value % 10 + '0'); value /= 10;
            buffer[12] = '.';

            int length = 9, decimalLength = 8;
            do {
                buffer[20 - length++] = (char)(value % 10 + '0');
                value /= 10;
            } while (value != 0);

            while (buffer[12 + decimalLength] == '0') decimalLength--;
            if (decimalLength == 0) decimalLength = -1;

            return new string(buffer, 21 - length, length + decimalLength - 8);
        }
    }
}
