using ManagedCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces.Rpc
{
    public interface IScummWebServerRpc : IDisposable
    {
        Task InitAsync(string gameId);
        Task RunGameAsync(AvailableGames gameName, string signalrConnectionId, string compressedAndEncodedGameSaveData);
        Task QuitAsync(string signalrConnectionId);
        string GetControlKeys();
    }
}
