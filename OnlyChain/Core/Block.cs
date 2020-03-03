using System;
using System.Collections.Generic;
using System.Text;

namespace OnlyChain.Core {
    public class Block {
        public uint Version { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Hash<Size256> HashPrevBlock { get; private set; }
        public Hash<Size256> HashMerkleRoot { get; private set; }
        public Address Producer { get; private set; }
        public Hash<Size256> Hash { get; private set; }
        public uint Height { get; private set; }

    }
}
