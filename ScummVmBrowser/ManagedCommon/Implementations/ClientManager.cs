using ManagedCommon.Interfaces.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Implementations
{
   public class RpcClientManager<TKey,TValue>: Dictionary<TKey,TValue>, IRpcClientProxyRemover<TKey>
    {
        
    }
}
