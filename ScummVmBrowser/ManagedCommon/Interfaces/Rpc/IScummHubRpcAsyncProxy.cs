using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces.Rpc
{
    public interface IScummHubRpcAsyncProxy: IScummHubRpc, IRpcClientService<IScummHubClientAsyncRpc>
    {
    }
}
