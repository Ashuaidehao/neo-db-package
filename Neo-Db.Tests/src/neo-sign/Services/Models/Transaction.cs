using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Services.Models
{
    public class Transaction
    {
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public string Decimals { get; set; }
    }
}
