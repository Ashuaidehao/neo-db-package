using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Rpc
{
    /// <summary>
    /// Error codes for different Rpc errors
    /// -32000 to -32099:Server error,Reserved for implementation-defined server-errors.
    /// </summary>
    public enum RpcErrorCode : short
    {                     
        ParseError = -32700,
        InvalidRequest = -32600,
        MethodNotFound = -32601,
        InvalidParams = -32602,
        InternalError = -32603
    }
}
