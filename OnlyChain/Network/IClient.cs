using OnlyChain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlyChain.Network {
    public interface IClient {
        string NetworkPrefix { get; }
        Address Address { get; }
        KBucket Nodes { get; }
    }
}
