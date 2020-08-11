using ManagedCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
   public interface IScummHubRpc: IDisposable
    {
        Task RunGameAsync(AvailableGames gameName, Dictionary<string,byte[]> gameStorage);
        void Quit();
    }
}
