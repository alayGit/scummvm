using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using ManagedCommon.Interfaces;
using ScummVMBrowser.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowerTests
{
    public class MockScummVMHubClient : IScummVMHubClient
    {
        public bool GameStarted { get; private set; }
        public bool ConnectionStarted { get; private set; }
        public bool HasFrontEndQuit { get; private set; }

        public List<string> EnqueuedStrings { get; set; }
        public List<ControlKeys> EnqueuedControlKeys { get; set; }


        public MockScummVMHubClient()
        {
            GameStarted = false;
            ConnectionStarted = false;
            HasFrontEndQuit = false;

            EnqueuedStrings = new List<string>();
            EnqueuedControlKeys = new List<ControlKeys>();
        }

        public CopyRectToScreenAsync ScreenDrawingCallback { get; set; }

        public AvailableGames GameName => throw new NotImplementedException();

        public bool HasConnectionStarted => throw new NotImplementedException();

        public bool HasGameStarted => GameStarted;

        public bool IsGameRunning => GameStarted && !HasFrontEndQuit;

        public int RpcPort => throw new NotImplementedException();

        public event Quit OnQuit;

        public Task BeforeDispose()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task EnqueueControlKey(ControlKeys toSend)
        {
            EnqueuedControlKeys.Add(toSend);

            return Task.CompletedTask;
        }

        public Task EnqueueString(string toSend)
        {
            EnqueuedStrings.Add(toSend);

            return Task.CompletedTask;
        }

        public Task Quit()
        {
            HasFrontEndQuit = true;

            return Task.CompletedTask;
        }

        public void SetNextFrameFunctionPointer(CopyRectToScreenAsync screenDrawingCallback)
        {
            ScreenDrawingCallback = screenDrawingCallback;
        }

        public Task Init()
        {
            ConnectionStarted = true;

            return Task.CompletedTask;
        }

        public Task StartGame(Dictionary<string,byte[]> gameStorage, AvailableGames gameName)
        {
            GameStarted = true;

            return Task.CompletedTask;
        }

        public void SetSaveGameFunctionPointer(SaveDataAsync saveGameCallback)
        {
           
        }
        public Task SendWholeScreenToNextFrameCallback()
        {
            return Task.CompletedTask;
        }

        public void SetPlaySoundFunctionPointer(PlayAudioAsync playSoundCallback)
        {
           
        }

        public Task StartSound()
        {
            throw new NotImplementedException();
        }

        public Task StopSound()
        {
            throw new NotImplementedException();
        }

        public Task EnqueueMouseMove(int x, int y)
        {
            throw new NotImplementedException();
        }

        public Task EnqueueMouseClick(MouseClick mouseClick)
        {
            throw new NotImplementedException();
        }

        public async Task StartConnection()
        {
            await Task.CompletedTask;
        }
    }
}
