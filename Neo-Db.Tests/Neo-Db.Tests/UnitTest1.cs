using System;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Neo;
using Neo.Wallets;
using Neo.Wallets.SQLite;
using Xunit;
using Xunit.Abstractions;

namespace Neo_Db.Tests
{
    public class UnitTest1: BaseTest
    {

        private WalletIndexer walletIndexer = new WalletIndexer("xunit_indexer");

        public UnitTest1(ITestOutputHelper output) : base(output)
        {
        }
        [Fact]
        public void Test1()
        {
            var wallet = UserWallet.Create(walletIndexer, "sqllite", "123456");
            WalletAccount account = wallet.CreateAccount();
            Print($"address: {account.Address}");
            Print($" pubkey: {account.GetKey().PublicKey.EncodePoint(true).ToHexString()}");
        }


    }
}
