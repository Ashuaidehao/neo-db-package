using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Neo.VM;

namespace Neo.Rpc
{
    public class RpcMethodResolver
    {
        private readonly IDictionary<string, MethodInfo> _dictionary = new Dictionary<string, MethodInfo>();



        public RpcMethodResolver RegisterAllAssemblies(IServiceCollection serviceCollection = null)
        {
            var markType = typeof(RpcMethodAttribute);
            var defineAssembly = Assembly.GetAssembly(markType);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var refers = assembly.GetReferencedAssemblies();
                if (assembly != defineAssembly && !refers.Contains(defineAssembly.GetName()))
                {
                    continue;
                }
                var exportTypes = assembly.GetExportedTypes();
                foreach (var exportType in exportTypes)
                {
                    bool needRegister = false;
                    foreach (var methodInfo in exportType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                    {
                        var rpcMethod = methodInfo.GetCustomAttribute<RpcMethodAttribute>();
                        if (rpcMethod != null)
                        {
                            _dictionary[rpcMethod.Method] = methodInfo;
                            needRegister = true;
                        }
                    }
                    if (serviceCollection != null && needRegister)
                    {
                        serviceCollection.AddScoped(exportType);
                    }
                }
            }

            return this;
        }


        public MethodInfo FindMethod(string method)
        {
            if (_dictionary.ContainsKey(method))
            {
                return _dictionary[method];
            }

            return null;
        }
    }
}
