using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.SmartContract;

namespace Neo.Wallets
{
    public class AccountModel
    {
        public UInt160 ScriptHash { get; set; }

        public Contract Contract { get; set; }

        public string Address { get; set; }

        public byte[] PrivateKey { get; set; }

        public KeyPair Key { get; set; }
        
    }
}
