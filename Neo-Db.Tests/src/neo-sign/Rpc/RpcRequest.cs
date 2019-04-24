using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Rpc
{
    public class RpcRequest
    {

        /// <summary>
        /// A String specifying the version of the JSON-RPC protocol. MUST be exactly "2.0"
        /// </summary>
        public string JsonRpc { get; set; }
     
        /// <summary>
        /// A String containing the name of the method to be invoked. Method names that begin with the word rpc followed by a period character (U+002E or ASCII 46) are reserved for rpc-internal methods and extensions and MUST NOT be used for anything else.
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// A Structured value that holds the parameter values to be used during the invocation of the method. This member MAY be omitted.
        /// </summary>
        public object Params { get; set; }
        /// <summary>
        ///An identifier established by the Client that MUST contain a String, Number, or NULL value if included. If it is not included it is assumed to be a notification. 
        /// </summary>
        public string Id { get; set; }
    }
}
