using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Wallets
{
    public interface IWallet
    {
        /// <summary>
        /// 根据地址获取私钥
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        byte[] GetPrivateKey(string address);


        AccountModel GetAccount(string address);
    }
}
