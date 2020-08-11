using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRHostWithUnity
{
    public interface IHubConnectionFactory
    {
        IHubProxyConnection CreateConnection(string url);
    }
}
