using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Rpc
{
    public class RpcMethodAttribute : Attribute
    {
        public string Method { get; }

        public RpcMethodAttribute(string method)
        {
            Method = method;
        }
    }
}
