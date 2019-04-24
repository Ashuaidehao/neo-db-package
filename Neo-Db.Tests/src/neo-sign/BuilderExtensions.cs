using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Neo.Rpc;

namespace Neo
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// Extension method to add the JsonRpc router services to the IoC container
        /// </summary>
        /// <param name="serviceCollection">IoC serivce container to register JsonRpc dependencies</param>
        /// <returns>IoC service container</returns>
        public static IServiceCollection AddNeoJsonRpc(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            //var markType = typeof(RpcMethodAttribute);
            //var defineAssembly = Assembly.GetAssembly(markType);
            //var map = new Dictionary<string, (MethodInfo methodInfo,ParameterInfo[] parameterInfos)>();

            //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    var refers = assembly.GetReferencedAssemblies();
            //    if (assembly != defineAssembly && !refers.Contains(defineAssembly.GetName()))
            //    {
            //        continue;
            //    }
            //    var exportTypes = assembly.GetExportedTypes();
            //    foreach (var exportType in exportTypes)
            //    {
            //        foreach (var methodInfo in exportType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            //        {
            //            var rpcMethod = methodInfo.GetCustomAttribute<RpcMethodAttribute>();
            //            if (rpcMethod != null)
            //            {
            //                map[rpcMethod.Method] = (methodInfo,methodInfo.GetParameters());
            //            }
            //        }
            //    }
            //}

            serviceCollection.AddSingleton(new RpcMethodResolver().RegisterAllAssemblies());
            serviceCollection.AddSingleton<JsonRpcHandler>();
            serviceCollection.AddSingleton<JsonRpcMiddleware>();

            //serviceCollection.AddSingleton(new RpcServicesMarker());
            //serviceCollection
            //    .TryAddScoped<IRpcInvoker, DefaultRpcInvoker>();
            //serviceCollection
            //    .TryAddScoped<IRpcParser, DefaultRpcParser>();
            //serviceCollection
            //    .TryAddScoped<IRpcRequestHandler, RpcRequestHandler>();
            //serviceCollection
            //    .TryAddScoped<IStreamCompressor, DefaultStreamCompressor>();
            //serviceCollection
            //    .TryAddScoped<IRpcResponseSerializer, DefaultRpcResponseSerializer>();
            //serviceCollection
            //    .TryAddScoped<IRpcRouteProvider, RpcAutoRouteProvider>();
            //serviceCollection
            //    .TryAddScoped<IRpcRequestMatcher, DefaultRequestMatcher>();



            return serviceCollection;
        }
    }
}
