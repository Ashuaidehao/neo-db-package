using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNep5Tracker.DB
{
    public class TrackDB
    {

        private SQLiteContext _db = new SQLiteContext("test.db");
        public void Add(Nep5TransferInfo newTransaction)
        {
            var tran = new Nep5TransactionEntity
            {
                BlockHeight = newTransaction.BlockHeight,
                TxId = newTransaction.TxId.ToHexString(),
                From = newTransaction.From.ToHexString(),
                To = newTransaction.To.ToHexString(),
                FromBalance = newTransaction.FromBalance.ToByteArray(),
                ToBalance = newTransaction.ToBalance.ToByteArray(),
                Amount = newTransaction.Amount.ToByteArray(),
                AssetId = newTransaction.AssetId.ToHexString(),
                Time = newTransaction.TimeStamp.FromTimestampMS(),
            };
            _db.Nep5Transactions.Add(tran);
            _db.SaveChanges();
        }
    }
}
