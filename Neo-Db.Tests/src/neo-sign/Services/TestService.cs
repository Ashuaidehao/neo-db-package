using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.Cryptography;
using Neo.Network.RPC;
using Neo.Rpc;
using Neo.SmartContract;
using Neo.Wallets;
using Neo.Wallets.SQLite;

namespace Neo.Services
{
    public class TestService
    {
        [RpcMethod("test1")]
        public async Task<string> SignAsync(string txData, string privateKey)
        {
            var data = txData.HexToBytes();
            var keypair = new KeyPair(privateKey.HexToBytes());
            var signData = Crypto.Default.Sign(data, keypair.PrivateKey, keypair.PublicKey.EncodePoint(false).Skip(1).ToArray());
            return signData.ToHexString();
        }


        [RpcMethod("test2")]
        public async Task<object> Test2(Para input)
        {
            return input;
        }

        [RpcMethod("test3")]
        public async Task<object> Test3(string name, int age)
        {
            return new { name, age };
        }

        [RpcMethod("test")]
        public async Task<object> TestAsync(string address)
        {
            //var wallet = new Nep6CoreWallet("debug-wallet.json", "123456");
            //await wallet.GetAccount(address);


            var db = new SqlLiteWallet("debug.db3", "123456");
            var account = await db.GetAccount(address);
            if (account == null)
            {
                throw new RpcErrorException(RpcErrorCode.InvalidRequest,"address not found");
            }

            return account;
        }


        private string ConvertToAddress(byte[] privateKey)
        {
            var key = new KeyPair(privateKey);
            var script = Contract.CreateSignatureRedeemScript(key.PublicKey);
            var sh = script.ToScriptHash();
            var address = sh.ToAddress();
            return address;
        }
    }

    public class Para
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
