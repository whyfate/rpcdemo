using Rpc.Common;
using Rpc.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rpc.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            RpcClient.SetTarget<IHelloService>("127.0.0.1", 12345);
            var tasks = new List<Task>();
            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var helloService = RpcClient.Create<IHelloService>();
                    var str = helloService.Get();
                    Console.WriteLine(str);
                }));
            }
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();
            Console.WriteLine("耗时:" + stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
