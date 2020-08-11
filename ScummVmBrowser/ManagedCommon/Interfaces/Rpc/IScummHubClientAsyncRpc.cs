using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces.Rpc
{
   public interface IScummHubClientAsyncRpc
    {
        Task<bool> SaveGameAsync(byte[] saveData, string fileName);
        Task BackEndQuitAsync();
    }
}
