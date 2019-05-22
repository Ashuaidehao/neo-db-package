using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Neo.Rpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Neo
{
    public class JsonRpcMiddleware : IMiddleware
    {
        private readonly JsonRpcHandler _handler;

        private IRpcSerializer _serializer = new RpcSerializer();

        public JsonRpcMiddleware(JsonRpcHandler handler)
        {
            _handler = handler;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST";
            context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
            context.Response.Headers["Access-Control-Max-Age"] = "31536000";
            if (context.Request.Method != "GET" && context.Request.Method != "POST") return;

            RpcRequest request = new RpcRequest();
            List<RpcRequest> requests = null;
            if (context.Request.Method == "GET")
            {
                string jsonrpc = context.Request.Query["jsonrpc"];
                string id = context.Request.Query["id"];
                string method = context.Request.Query["method"];
                string _params = context.Request.Query["params"];
                if (id.NotNull() || method.NotNull() || _params.NotNull())
                {
                    try
                    {
                        _params = Encoding.UTF8.GetString(Convert.FromBase64String(_params));
                    }
                    catch (FormatException) { }

                    if (jsonrpc.NotNull())
                    {
                        request.JsonRpc = jsonrpc;
                    }
                    request.Id = id;
                    request.Method = method;
                    request.Params = JObject.Parse(_params);
                }
            }
            else if (context.Request.Method == "POST")
            {
                using (StreamReader reader = new StreamReader(context.Request.Body))
                {
                    try
                    {
                        var rawRequest = JsonConvert.DeserializeObject<JToken>(await reader.ReadToEndAsync());
                        if (rawRequest.Type == JTokenType.Array)
                        {
                            // batch request
                            requests = rawRequest.ToObject<List<RpcRequest>>();
                        }
                        else
                        {
                            // single request
                            request = rawRequest.ToObject<RpcRequest>();
                        }

                    }
                    catch (FormatException) { }
                }
            }

            object response = null;

            if (requests != null && requests.Any())
            {
                var tasks = requests.Select(ProcessRequest);
                var batchResponse = await Task.WhenAll(tasks);
                response = batchResponse;
            }
            else
            {
                var singleResponse = await ProcessRequest(request);
                response = singleResponse;
            }
            context.Response.ContentType = "application/json-rpc";
            await context.Response.WriteAsync(_serializer.Serialize(response), Encoding.UTF8);
        }


        public async Task<RpcResponse> ProcessRequest(RpcRequest request)
        {
            var response = new RpcResponse(request.Id);
            try
            {
                response = await _handler.InvokeAsync(request);
            }
            catch (RpcErrorException rpcException)
            {
                response.Error = new RpcError(rpcException.Code, rpcException.Message) { Data = new { Stack = rpcException.StackTrace } };
            }
            catch (Exception ex)
            {
                response.Error = new RpcError(RpcErrorCode.InternalError, ex.Message) { Data = new { Stack = ex.StackTrace } };
            }
            return response;
        }
    }
}
