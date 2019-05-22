using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Neo.Services;
using Neo.SmartContract;
using Newtonsoft.Json;

namespace Neo.Wallets
{
    public class Nep6CoreWallet:IWallet
    {
        private string _path;

        private string _password;

        private Nep6Object _db;
        public Nep6CoreWallet(string path, string password)
        {
            _path = path;
            _password = password;
            if (File.Exists(path))
            {
                _db = JsonConvert.DeserializeObject<Nep6Object>(File.ReadAllText(path));
            }
        }

        public byte[] GetPrivateKey(string address)
        {
            var account = _db.accounts.FirstOrDefault(ac => ac.address == address);
            if (account != null && VerifyPassword(account, _password, out var privateKey))
            {
                return privateKey;
            }
            return null;
        }

        public AccountModel GetAccount(string address)
        {
            var account = _db.accounts.FirstOrDefault(ac => ac.address == address);
            if (account != null && VerifyPassword(account, _password, out var privateKey))
            {
                var key=new KeyPair(privateKey);
                return new AccountModel()
                {
                    Address = address,
                    PrivateKey = privateKey,
                    ScriptHash = address.ToScriptHash(),
                    Key = key,
                    Contract = Contract.CreateSignatureContract(key.PublicKey)
                };
            }

            return null;
        }




        internal bool VerifyPassword(AccountObject account, string password,out byte[] privateKey)
        {
            try
            {
                privateKey= Wallet.GetPrivateKeyFromNEP2(account.key, password, _db.scrypt.n, _db.scrypt.r, _db.scrypt.p);
                return true;
            }
            catch (FormatException)
            {
                privateKey = null;
                return false;
            }
        }
    }

    internal class ScryptObject
    {
        public int n { get; set; }
        public int r { get; set; }
        public int p { get; set; }
    }

    internal class ParameterObject
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    internal class ContractObject
    {
        public string script { get; set; }
        public List<ParameterObject> parameters { get; set; }
        public bool deployed { get; set; }
    }

    internal class AccountObject
    {
        public string address { get; set; }
        public object label { get; set; }
        public bool isDefault { get; set; }
        public bool @lock { get; set; }
        public string key { get; set; }
        public ContractObject contract { get; set; }
        public object extra { get; set; }
    }

    internal class Nep6Object
    {
        public object name { get; set; }
        public string version { get; set; }
        public ScryptObject scrypt { get; set; }
        public List<AccountObject> accounts { get; set; }
        public object extra { get; set; }
    }

}