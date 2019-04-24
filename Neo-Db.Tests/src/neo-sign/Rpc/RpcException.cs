using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Rpc
{
    public class RpcException : Exception
    {
        public RpcException(RpcErrorCode code, string message) : base(message)
        {
            Code = code;
        }

        public RpcErrorCode Code { get; set; }
    }
}
