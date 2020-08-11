using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRHostWithUnity
{
    public class HubProxyConnection : HubConnection, IHubProxyConnection
    {
        internal HubProxyConnection(string url):base(url)
        { }
    }
}
