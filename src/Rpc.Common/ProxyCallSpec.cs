using System;
using System.Collections.Generic;
using System.Text;

namespace Rpc.Common
{
    internal class ProxyCallSpec
    {
        public string TypeFullName { get; set; }

        public string MethodName { get; set; }

        public object[] Params { get; set; }
    }
}
