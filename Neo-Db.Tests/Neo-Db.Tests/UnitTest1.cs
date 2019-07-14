using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Neo;
using Neo.Wallets;
using Neo.Wallets.SQLite;
using Xunit;
using Xunit.Abstractions;

namespace Neo_Db.Tests
{
    public class UnitTest1 : BaseTest
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

        [Fact]
        public void Reverse()
        {
            //×ª»»Ð¡¶ËÐò
            var neoId = "c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b";
            var reversedneoId = neoId.HexToBytes().Reverse().ToHexString();
            Print(reversedneoId);

        }

        [Fact]
        public void Test2()
        {

            var text = new string('0', 63);

            var a = new HashSet<UInt256>
            {

            };
            for (int i = 1; i < 5; i++)
            {
                a.Add(new UInt256((text + i).HexToBytes()));
            }
            var b = new HashSet<UInt256>
            {

            };
            for (int i = 2; i < 5; i++)
            {
                b.Add(new UInt256((text + i).HexToBytes()));

            }




            a.ExceptWith(b);

            foreach (var uInt256 in a)
            {
                Print(uInt256);
            }

        }
    }
}
