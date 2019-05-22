using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Rpc
{
    public class RpcErrorException : Exception
    {
        public RpcErrorException(RpcErrorCode code, string message) : base(message)
        {
            Code = code;
        }

        public RpcErrorCode Code { get; set; }
    }
}
