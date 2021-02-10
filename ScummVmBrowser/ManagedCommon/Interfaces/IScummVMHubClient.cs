using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
    public interface IScummVMHubClient : IDisposable
    {
        AvailableGames GameName { get; }
        bool HasGameStarted { get; }
        bool HasFrontEndQuit { get; }
        
        event Quit OnQuit;
        bool IsGameRunning { get; }
        int RpcPort { get; }

        Task BeforeDispose();
        void Dispose();
        Task EnqueueControlKey(ControlKeys toSend);
        Task Init();
        Task StartConnection();
        Task EnqueueMouseMove(int x, int y);
        Task EnqueueMouseClick(MouseClick mouseClick);
        Task EnqueueString(string toSend);
        Task Quit();
        void SetNextFrameFunctionPointer(CopyRectToScreenAsync screenDrawingCallback);
        void SetSaveGameFunctionPointer(SaveDataAsync saveGameCallback);
        Task StartGame(Dictionary<string, byte[]> gameStorage, AvailableGames gameName);
        Task SendWholeScreenToNextFrameCallback();

        Task StartSound();
        Task StopSound();
    }
}
