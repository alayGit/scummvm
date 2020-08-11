using SignalRHostWithUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowerTests
{
    public class MockHubConnectionFactory : IHubConnectionFactory
    {
        public IHubProxyConnection HubProxyConnection { get; set; }
        public MockHubConnectionFactory(IHubProxyConnection hubProxyConnection)
        {
            HubProxyConnection = hubProxyConnection;
        }

        public IHubProxyConnection CreateConnection(string url)
        {
            return HubProxyConnection;  
        }
    }
}
