using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRHostWithUnity
{
    public interface IHubProxyConnection: IConnection, IDisposable
    {
        IHubProxy CreateHubProxy(string hubName);
        Task Start();
    }
}
