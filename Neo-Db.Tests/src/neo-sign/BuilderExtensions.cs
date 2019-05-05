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

            serviceCollection.AddSingleton(new RpcMethodResolver().RegisterAllAssemblies());
            serviceCollection.AddSingleton<JsonRpcHandler>();
            serviceCollection.AddSingleton<JsonRpcMiddleware>();

            return serviceCollection;
        }
    }
}
