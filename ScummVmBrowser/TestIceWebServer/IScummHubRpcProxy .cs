using ManagedCommon.Interfaces;
using ManagedCommon.Interfaces.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc
{
    public interface IScummWebServerRpcProxy: IScummHubRpc, IRpcClientService<IScummWebServerClientRpc>
    {
    }
}
