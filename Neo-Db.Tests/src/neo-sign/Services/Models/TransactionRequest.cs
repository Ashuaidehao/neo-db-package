using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.Network.P2P.Payloads;

namespace Neo.Services.Models
{
    public class TransactionRequest
    {
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public string Decimals { get; set; }

        public TransactionType TransactionType { get; set; }

        public string Version { get; set; }

        public TransactionAttribute[] TransactionAttribute { get; set; }

        public InputCoinRequest[] Inputs { get; set; }
        public OutoutCoinRequest[] Outputs { get; set; }

        /// <summary>
        /// 收款地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 转账金额
        /// </summary>
        public string Amount { get; set; }

        ///// <summary>
        ///// 私钥
        ///// </summary>
        //public string PrivateKey { get; set; }
    }

    public class InputCoinRequest
    {
        /// <summary>
        /// 指向的UTXO所在的交易的hash值
        /// </summary>
        public string PrevHash { get; set; }
        /// <summary>
        /// 指向的UTXO所在的交易的output的位置。从0开始。
        /// </summary>
        public ushort PrevIndex { get; set; }
    }

    public class OutoutCoinRequest
    {
        public string Asset { get; set; }
        public string Value { get; set; }
        public string Address { get; set; }
    }
}
