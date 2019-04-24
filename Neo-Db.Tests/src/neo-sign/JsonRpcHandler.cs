using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Neo.Rpc;
using Neo.Services;
using Newtonsoft.Json.Linq;

namespace Neo
{
    public class JsonRpcHandler
    {
        private readonly IServiceProvider _provider;
        private readonly RpcMethodResolver _rpcMethodResolver;



        public JsonRpcHandler(IServiceProvider provider, RpcMethodResolver rpcMethodResolver)
        {
            _provider = provider;
            _rpcMethodResolver = rpcMethodResolver;


        }


        public async Task<RpcResponse> InvokeAsync(RpcRequest request)
        {
            var inputParams = request.Params as JToken;
            var methodInfo = _rpcMethodResolver.FindMethod(request.Method);
            if (methodInfo == null)
            {
                return new RpcResponse(request.Id).ToError(RpcErrorCode.MethodNotFound, "method not found");
            }
            var parameterInfos = methodInfo.GetParameters();

            var parameterValues = new List<object>();
            if (inputParams.Type == JTokenType.Array)
            {
                var count = inputParams.Count() < parameterInfos.Length ? inputParams.Count() : parameterInfos.Length;
                for (int i = 0; i < count; i++)
                {
                    parameterValues.Add(inputParams[i].ToObject(parameterInfos[i].ParameterType));
                }
            }
            else
            {
                if (parameterInfos.Length == 1)
                {
                    parameterValues.Add(inputParams.ToObject(parameterInfos[0].ParameterType));
                }
                else
                {
                    foreach (var parainfo in parameterInfos)
                    {
                        var p = inputParams.Value<JToken>(parainfo.Name).ToObject(parainfo.ParameterType);
                        parameterValues.Add(p);
                    }
                }
            }

            var instance = _provider.GetService(methodInfo.DeclaringType) ??
                           Activator.CreateInstance(methodInfo.DeclaringType);

            var result = await (dynamic)methodInfo.Invoke(instance, parameterValues.ToArray());
            return new RpcResponse(request.Id)
            {
                Result = result
            };
        }
    }
}
