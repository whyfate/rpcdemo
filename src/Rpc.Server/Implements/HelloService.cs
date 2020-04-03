using Rpc.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rpc.Server.Implements
{
    public class HelloService : IHelloService
    {
        public string Get()
        {
            return "get by server";
        }
    }
}
