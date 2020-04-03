using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Linq;

namespace Rpc.Common
{
    public class RpcClient
    {
        private static Dictionary<IPEndPoint, List<Type>>
            dictionary = new Dictionary<IPEndPoint, List<Type>>();
        public static void SetTarget<T>(string host, int port)
        {
            var key = new IPEndPoint(IPAddress.Parse(host), port);
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<Type>());
            }

            dictionary[key].Add(typeof(T));
        }

        internal static EndPoint GetEndPoint(Type type)
        {
            return dictionary.FirstOrDefault(d => d.Value.Contains(type)).Key;
        }

        public static T Create<T>()
        {
            return DispatchProxy.Create<T, DispatcherProxy>();
        }
    }
}
