using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.Cryptography;
using Neo.Rpc;
using Neo.Wallets;

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

        [RpcMethod("test")]
        public async Task<object> Test(string txData)
        {
            return new { reverse =new string(txData.Reverse().ToArray()) };
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

    }

    public class Para
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
