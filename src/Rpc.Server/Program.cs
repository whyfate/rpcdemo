using Rpc.Common;
using Rpc.Server.Implements;
using Rpc.Service;
using System;

namespace Rpc.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new RpcServer("127.0.0.1", 12345)
                .Map<IHelloService, HelloService>();
            Console.WriteLine("开始接收请求...");
            server.Run();
        }
    }
}
