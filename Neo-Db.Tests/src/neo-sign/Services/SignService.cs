using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anemonis.AspNetCore.JsonRpc;
using Neo.Cryptography;
using Neo.Wallets;

namespace Neo.Services
{
    [JsonRpcRoute("/api")]
    public class SignService : IJsonRpcService
    {

        [JsonRpcMethod("createsign",0,1)]
        public async Task<string > SignAsync(string txData,string privateKey)
        {
            var data = txData.HexToBytes();
            var keypair = new KeyPair(privateKey.HexToBytes());
            var signData = Crypto.Default.Sign(data, keypair.PrivateKey, keypair.PublicKey.EncodePoint(false).Skip(1).ToArray());
            return signData.ToHexString();
        }

    }
}
