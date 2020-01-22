using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNep5Tracker.DB
{
    public class Nep5TransactionEntity
    {
        [Key]
        public long Id { get; set; }

        public uint BlockHeight { get; set; }

        public string TxId { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public byte[] FromBalance { get; set; }
        public byte[] ToBalance { get; set; }
        public byte[] Amount { get; set; }
        public string AssetId { get; set; }
        public DateTime Time { get; set; }
    }
}
