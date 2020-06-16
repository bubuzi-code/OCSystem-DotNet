#nullable enable

using OnlyChain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OnlyChain.Model {
    public abstract class AttachData {
        public static AttachData? ParseData(Transaction tx) {
            try {
                if (tx.To == Address.Zero) {
                    var deserializer = new Deserializer(tx.Data);
                    byte cmd = deserializer.Read<byte>();
                    if (cmd is 0x01) { // 投票，转到0地址，Data里指明投给哪些地址
                        uint round = deserializer.Read(Deserializer.VarUInt32);
                        int count = Math.DivRem(tx.Data.Length - deserializer.Index, Address.Size, out int r);
                        if (r != 0) goto Fail;

                        return new VoteData(round, deserializer.ReadValues<Address>(count));
                    } else if (tx.Data is { Length: 1 }) {
                        if (cmd is 0x02) return new VoteRedemptionData(); // 投票质押手动赎回
                        if (cmd is 0xff) return new SuperRedemptionData(); // 超级节点质押手动赎回
                    }
                } else if (tx.To == Address.Max) {
                    return new SuperPledgeData();
                } else {
                    return null;
                }
            } catch { }
        Fail:
            throw new FormatException();
        }
    }
}
