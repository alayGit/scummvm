using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRHostWithUnity
{
    public class HubConnectionFactory : IHubConnectionFactory
    {
        public IHubProxyConnection CreateConnection(string url)
        {
            return new HubProxyConnection(url);
        }
    }
}
