using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.Cryptography;
using Neo.Network.P2P.Payloads;
using Neo.Rpc;
using Neo.Services.Models;
using Neo.SmartContract;
using Neo.Wallets;

namespace Neo.Services
{
    public class SignService
    {

        [RpcMethod("signdata")]
        public async Task<string> SignAsync(string txData, string privateKey)
        {
            var data = txData.HexToBytes();
            var keypair = new KeyPair(privateKey.HexToBytes());
            var signData = Crypto.Default.Sign(data, keypair.PrivateKey, keypair.PublicKey.EncodePoint(false).Skip(1).ToArray());
            return signData.ToHexString();
        }



        [RpcMethod("signtx")]
        public async Task<string> SignAsync(TransactionRequest request)
        {
            var address = request.Address.ToScriptHash();
            Fixed8.TryParse(request.Amount, out Fixed8 amount);
            var tx = new ContractTransaction()
            {
                Attributes = new TransactionAttribute[0],
                Inputs = request.Inputs?.Select(c=>new CoinReference()
                {
                    PrevIndex = c.PrevIndex,
                    PrevHash = UInt256.Parse(c.PrevHash),
                }).ToArray(),
                Outputs = new[]
                {
                    new TransactionOutput
                    {
                        AssetId = UInt256.Parse(request.AssetId),
                        Value = amount,
                        ScriptHash = address
                    }
                }
            };
            var signData= tx.Sign(new KeyPair(request.PrivateKey.HexToBytes()));
            return signData.ToHexString();
        }

    }
}
