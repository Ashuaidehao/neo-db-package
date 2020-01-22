using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Neo;

namespace MyNep5Tracker
{
    public class Nep5TransferInfo
    {
        public uint BlockHeight { get; set; }

        public UInt256 TxId { get; set; }
        public UInt160 From { get; set; }
        public UInt160 To { get; set; }

        public BigInteger FromBalance { get; set; }
        public BigInteger ToBalance { get; set; }
        public BigInteger Amount { get; set; }
        public UInt160 AssetId { get; set; }
        public ulong TimeStamp { get; set; }
    }
}
