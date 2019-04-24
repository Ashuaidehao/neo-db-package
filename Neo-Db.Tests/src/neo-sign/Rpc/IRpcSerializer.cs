using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Neo.Rpc
{
    public interface IRpcSerializer
    {
        string Serialize(object obj);
    }

    public class RpcSerializer : IRpcSerializer
    {

        private JsonSerializerSettings _settings=new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
        };
        public string Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(obj, _settings);
        }
    }
}
