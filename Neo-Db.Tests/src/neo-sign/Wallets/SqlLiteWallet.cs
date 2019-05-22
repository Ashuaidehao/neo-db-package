using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Neo.IO;
using Neo.Wallets.SQLite;

namespace Neo.Wallets
{
    public class SqlLiteWallet
    {
        private string _filepath;

        private byte[] _iv;
        private byte[] _masterKey;
        public SqlLiteWallet(string path, string password)
        {
            _filepath = path;
            _iv = LoadStoredData("IV");
            _masterKey = AesDecrypt(LoadStoredData("MasterKey"), ToAesKey(password), _iv);
        }


        private byte[] LoadStoredData(string name)
        {
            using (WalletDataContext ctx = new WalletDataContext(_filepath))
            {
                return ctx.Keys.FirstOrDefault(p => p.Name == name)?.Value;
            }
        }

        public async Task<AccountModel> GetAccount(string address)
        {
            using (WalletDataContext ctx = new WalletDataContext(_filepath))
            {
                var addressScriptHash = address.ToScriptHash();
                var contractEntity = ctx.Contracts.Include(a => a.Account).FirstOrDefault(a => a.ScriptHash == addressScriptHash.ToArray());
                var account = contractEntity?.Account;
                if (account == null)
                {
                    return null;
                }
                VerificationContract contract = contractEntity.RawData.AsSerializable<VerificationContract>();

                var accountModel = new AccountModel()
                {
                    Address = address,
                    Contract = contract,
                    ScriptHash = addressScriptHash,
                    PrivateKey = DecryptPrivateKey(account.PrivateKeyEncrypted),
                };
                return accountModel;
            }
        }

        /// <summary>
        /// 解密私钥，并计算公私钥对
        /// </summary>
        /// <param name="encryptedPrivateKey"></param>
        /// <returns></returns>
        public KeyPair GetKeyPair(byte[] encryptedPrivateKey)
        {
            var privateKey = DecryptPrivateKey(encryptedPrivateKey);
            return new KeyPair(privateKey);
        }


        /// <summary>
        /// 解密私钥
        /// </summary>
        /// <param name="encryptedPrivateKey"></param>
        /// <returns></returns>
        private byte[] DecryptPrivateKey(byte[] encryptedPrivateKey)
        {
            if (encryptedPrivateKey == null) throw new ArgumentNullException(nameof(encryptedPrivateKey));
            if (encryptedPrivateKey.Length != 96) throw new ArgumentException();
            return AesDecrypt(encryptedPrivateKey, _masterKey, _iv);
        }




        internal static byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || key == null || iv == null) throw new ArgumentNullException();
            if (data.Length % 16 != 0 || key.Length != 32 || iv.Length != 16) throw new ArgumentException();
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.None;
                using (ICryptoTransform decryptor = aes.CreateDecryptor(key, iv))
                {
                    return decryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        internal static byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || key == null || iv == null) throw new ArgumentNullException();
            if (data.Length % 16 != 0 || key.Length != 32 || iv.Length != 16) throw new ArgumentException();
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.None;
                using (ICryptoTransform encryptor = aes.CreateEncryptor(key, iv))
                {
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        internal static byte[] ToAesKey(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                //return sha256.ComputeHash(Encoding.UTF8.GetBytes(password), 2);

                byte[] buffer = Encoding.UTF8.GetBytes(password);
                for (int i = 0; i < 2; i++)
                {
                    buffer = sha256.ComputeHash(buffer);
                }

                return buffer;
            }
        }

        internal static byte[] ToAesKey1(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] passwordHash = sha256.ComputeHash(passwordBytes);
                byte[] passwordHash2 = sha256.ComputeHash(passwordHash);
                Array.Clear(passwordBytes, 0, passwordBytes.Length);
                Array.Clear(passwordHash, 0, passwordHash.Length);
                return passwordHash2;
            }
        }

    }



}
