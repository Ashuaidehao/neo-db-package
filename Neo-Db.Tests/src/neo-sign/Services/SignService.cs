using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.Cryptography;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Rpc;
using Neo.Services.Models;
using Neo.SmartContract;
using Neo.Wallets;

namespace Neo.Services
{
    public class SignService
    {
        private IWallet _wallet;

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
            _wallet = new Nep6CoreWallet("debug-wallet.json", "123456");

            var address = request.Address.ToScriptHash();
            Fixed8.TryParse(request.Amount, out Fixed8 amount);
            var tx = new ContractTransaction()
            {
                Attributes = new TransactionAttribute[0],
                Inputs = request.Inputs?.Select(c => new CoinReference()
                {
                    PrevIndex = c.PrevIndex,
                    PrevHash = UInt256.Parse(c.PrevHash),
                }).ToArray(),
                Outputs = request.Outputs?.Select(o=>new TransactionOutput
                    {
                        AssetId = UInt256.Parse(request.AssetId),
                        Value = Fixed8.Parse(o.Value),
                        ScriptHash = o.Address.ToScriptHash(),
                    }
                ).ToArray(),
            };
            var account = _wallet.GetAccount(request.Address);
            var keypair = new KeyPair(account.PrivateKey);
            var signData = tx.Sign(keypair);

            var context = new ContractParametersContext(tx);
            context.AddSign(account.Contract, keypair.PublicKey, signData);
            tx.Witnesses = context.GetWitness(new List<UInt160>(){address});
            using (var ms = new MemoryStream())
            using (var bi = new BinaryWriter(ms))
            {
                ((ISerializable)tx).Serialize(bi);

                var data = ms.ToArray();
                var text = data.ToHexString();
                var json = tx.ToJson();
                Console.WriteLine(json);
                Console.WriteLine(text);
            }
            return signData.ToHexString();
        }



    }
}
