using ManagedCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
    public interface IRpcServerService<R>: IDisposable
    {
        void Init(string hubName, string clientCallbackProxyName, string host, string id, R receiver, Protocol protocol);
    }
}
