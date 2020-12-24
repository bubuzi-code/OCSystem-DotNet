﻿using OnlyChain.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OnlyChain.Network.Objects {
    public sealed class BUInt : BValue<ulong> {
        public const byte PrefixChar = (byte)'u';

        public BUInt(ulong value) : base(value) {
        }

        protected override void Write(Stream stream) {
            stream.WriteByte(PrefixChar);
            stream.WriteVarUInt(Value);
        }

        public static implicit operator BUInt(ulong value) => new BUInt(value);
    }
}
