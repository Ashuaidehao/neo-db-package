using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Rpc
{
    public class RpcResponse
    {

        public RpcResponse(string id)
        {
            Id = id;
        }

        /// <summary>
        /// A String specifying the version of the JSON-RPC protocol. MUST be exactly "2.0"
        /// </summary>
        public string JsonRpc { get; set; } = "2.0";


        /// <summary>
        /// This member is REQUIRED on success.
        /// This member MUST NOT exist if there was an error invoking the method.
        /// The value of this member is determined by the method invoked on the Server.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// This member is REQUIRED on error.
        /// This member MUST NOT exist if there was no error triggered during invocation.
        /// </summary>
        public RpcError Error { get; set; }


        /// <summary>
        /// This member is REQUIRED.
        /// It MUST be the same as the value of the id member in the Request Object.
        /// If there was an error in detecting the id in the Request object (e.g.Parse error/Invalid Request), it MUST be Null.
        /// </summary>
        public string Id { get; set; }

        public RpcResponse ToError(RpcErrorCode code, string message = null)
        {
            Error = new RpcError(code, message);
            return this;
        }
    }

    /// <summary>
    /// When a rpc call encounters an error, the Response Object MUST contain the error member 
    /// </summary>
    public class RpcError
    {

        public RpcError(RpcErrorCode code, string message = null) : this((int)code, message)
        {
        }
        public RpcError(int code, string message = null)
        {
            Code = code;
            Message = message;
        }


        /// <summary>
        /// A Number that indicates the error type that occurred.
        /// This MUST be an integer.
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// A String providing a short description of the error.
        /// The message SHOULD be limited to a concise single sentence.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// A Primitive or Structured value that contains additional information about the error.
        /// This may be omitted.
        /// The value of this member is defined by the Server (e.g.detailed error information, nested errors etc.).
        /// </summary>
        public object Data { get; set; }
    }
}
