using System;
using System.Collections.Generic;
using System.Text;

namespace Rpc.Common
{
    internal class ProxyBackSpec
    {
        public bool Success { get; set; }

        public object Data { get; set; }

        public static ProxyBackSpec Successful(object data)
        {
            return new ProxyBackSpec
            {
                Success = true,
                Data = data
            };
        }

        public static ProxyBackSpec Fail()
        {
            return new ProxyBackSpec();
        }
    }
}
