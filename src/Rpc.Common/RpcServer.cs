using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Rpc.Common
{
    public class RpcServer
    {
        Socket socket;
        IServiceProvider serviceProvider;
        IServiceCollection services;
        Dictionary<string, Type> types = new Dictionary<string, Type>();

        public RpcServer(string host, int port)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                host = "127.0.0.1";
            }

            if (port < 0 || port > 65535)
            {
                throw new Exception("port error");
            }

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(host), port));
            services = new ServiceCollection();
        }

        public RpcServer Map<TI, TM>()
            where TI : class
            where TM : class, TI
        {
            var type = typeof(TI);
            types.Add(type.FullName, type);
            services.AddSingleton<TI, TM>();

            return this;
        }

        public RpcServer MapTransient<TI, TM>()
            where TI : class
            where TM : class, TI
        {
            var type = typeof(TI);
            types.Add(type.FullName, type);
            services.AddTransient<TI, TM>();

            return this;
        }

        public void Run()
        {
            serviceProvider = services.BuildServiceProvider();
            socket.Listen(20);
            while (true)
            {
                var clientSocket = socket.Accept();

                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Request), clientSocket);
            }
        }

        private void Request(object socketObj)
        {
            var clientSocket = (Socket)socketObj;
            var buffer = new byte[1024 * 1024];
            var len = clientSocket.Receive(buffer);
            var json = Encoding.UTF8.GetString(buffer, 0, len);
            var callObj = JsonConvert.DeserializeObject<ProxyCallSpec>(json);
            if (types.TryGetValue(callObj.TypeFullName, out Type type))
            {
                var service = serviceProvider.GetService(type);
                if (service != null)
                {
                    var result = type.GetMethod(callObj.MethodName)
                        .Invoke(service, callObj.Params);
                    Response(clientSocket, ProxyBackSpec.Successful(result));

                    return;
                }
            }

            Response(clientSocket, ProxyBackSpec.Fail());
        }

        private void Response(Socket clientSocket, ProxyBackSpec backSpec)
        {
            clientSocket.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(backSpec)));
            clientSocket.Close();
        }
    }
}
