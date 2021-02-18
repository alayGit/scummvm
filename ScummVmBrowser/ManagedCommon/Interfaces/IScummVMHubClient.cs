using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using ManagedCommon.Models;
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
        Task Init();
        Task StartConnection();
		Task EnqueueInputMessages(KeyValuePair<string, string>[] inputMessages);
        Task Quit();
        void SetSendGameMessagesFunctionPointer(SendGameMessagesAsync screenDrawingCallback);
        void SetSaveGameFunctionPointer(SaveDataAsync saveGameCallback);
        Task StartGame(Dictionary<string, byte[]> gameStorage, AvailableGames gameName);
        Task ScheduleRedrawWholeScreen();

        Task StartSound();
        Task StopSound();
    }
}
