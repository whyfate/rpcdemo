using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Rpc.Common
{
    public class DispatcherProxy : DispatchProxy
    {
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var spec = new ProxyCallSpec
            {
                TypeFullName = targetMethod.DeclaringType.FullName,
                MethodName = targetMethod.Name,
                Params = args
            };

            var specJson = Newtonsoft.Json.JsonConvert.SerializeObject(spec);

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            clientSocket.Connect(RpcClient.GetEndPoint(targetMethod.DeclaringType));
            var sendFlag = clientSocket.Send(Encoding.UTF8.GetBytes(specJson));
            var reveiveBuffer = new byte[1024 * 1024];
            var len = clientSocket.Receive(reveiveBuffer);
            clientSocket.Close();
            var receive = Newtonsoft.Json.JsonConvert.DeserializeObject<ProxyBackSpec>(Encoding.UTF8.GetString(reveiveBuffer, 0, len));
            if (receive.Success)
            {
                return receive.Data;
            }
            else
            {
                throw new Exception("call error");
            }
        }
    }
}
