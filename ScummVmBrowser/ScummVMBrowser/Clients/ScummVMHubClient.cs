using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ManagedCommon.Delegates;
using ScummVMBrowser.StaticData;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;
using Trasher;
using System.Threading;
using SharedMemory;
using ManagedCommon.ExtensionMethods;
using ManagedCommon.Base;
using ManagedCommon.Interfaces.Rpc;
using PortSharer;
using System.Threading.Tasks;

namespace ScummVMBrowser.Clients
{
    public class ScummVMHubClient : IScummVMHubClient, IScummHubClientAsyncRpc
    {
        public int RpcPort { get; private set; }
        public int RealTimePort { get; private set; }

        public AvailableGames GameName { get; private set; }
        public bool HasInited { get; private set; }
        public bool HasGameStarted { get; private set; }

        public event Quit OnQuit
        {
            add
            {
                try
                {
                    _fireQuitEventSemaphore.Wait();
                    _onQuit += value;
                }
                finally
                {
                    _fireQuitEventSemaphore.Release();
                }
            }
            remove
            {
                try
                {
                    _fireQuitEventSemaphore.Wait();
                    _onQuit -= value;
                }
                finally
                {
                    _fireQuitEventSemaphore.Release();
                }
            }
        }

        public bool IsGameRunning
        {
            get
            {
                return HasInited && HasGameStarted && !HasFrontEndQuit && !HasBackEndQuit;
            }
        }
        public bool HasFrontEndQuit { get; private set; }
        public bool HasBackEndQuit { get; private set; }

        private Quit _onQuit;
        private SaveDataAsync _saveDataCallback;
        private SemaphoreSlim _fireQuitEventSemaphore;
        private IRealTimeDataEndpointClient _realTimeDataEndpoint;
        private IScummHubRpcAsyncProxy _scummHubRpc;
        private IRealTimeDataBusClient _realTimeDataBusClient;
        private CopyRectToScreenAsync _screenDrawingCallback;
        private IConfigurationStore<System.Enum> _configurationStore;
        private string _rpcPortGetterId;
        private string _realTimePortGetterId;
        private IPortGetter _rpcPortGetter;
        private IPortGetter _realTimePortGetter;

        public ScummVMHubClient(IScummVMServerStarter starter, IConfigurationStore<System.Enum> configurationStore, IRealTimeDataEndpointClient realTimeDataEndpoint, IScummHubRpcAsyncProxy scummHubRpc, IRealTimeDataBusClient realTimeDataBusClient, IPortGetter rpcPortGetter, IPortGetter realTimePortGetter)
        {
            GameName = AvailableGames.kq3;
            HasInited = false;
            HasGameStarted = false;
            HasBackEndQuit = false;
            HasFrontEndQuit = false;
            _fireQuitEventSemaphore = new SemaphoreSlim(1);
            _realTimeDataEndpoint = realTimeDataEndpoint;
            _scummHubRpc = scummHubRpc;
            _realTimeDataBusClient = realTimeDataBusClient;
            _configurationStore = configurationStore;
            _rpcPortGetterId = Guid.NewGuid().ToString();
            _realTimePortGetterId = Guid.NewGuid().ToString();
            _realTimePortGetter = realTimePortGetter;
            _rpcPortGetter = rpcPortGetter;

            starter.StartScummVM(configurationStore.GetValue(ScummConnectionSettings.ScummVMLocation), _rpcPortGetterId, _realTimePortGetterId);
        }
        
        public async Task Init()
        {
            if (!HasInited)
            {
                try
                {
                    RpcPort = await _rpcPortGetter.GetPort(_rpcPortGetterId);
                    RealTimePort = await _realTimePortGetter.GetPort(_realTimePortGetterId);
                }
                finally
                {
                    //_realTimePortGetter.Dispose();
                    //_rpcPortGetter.Dispose();
                }

                _scummHubRpc.Init(_configurationStore.GetValue(ScummConnectionSettings.CliScummHubName), _configurationStore.GetValue(IceRemoteProcBackEnd.ClientName), _configurationStore.GetValue(ScummConnectionSettings.CliScummHubHostName), RpcPort, Guid.NewGuid().ToString(), this, Protocol.@default);
                await _realTimeDataBusClient.Init(RealTimePort.ToString());
                await _realTimeDataEndpoint.Init(RealTimePort.ToString());

                HasInited = true;
            }
            else
            {
                throw new Exception("The client is already inited");
            }
        }

        public async Task StartConnection()
        {
            await _realTimeDataBusClient.StartConnectionAsync();
        }


        public async Task StartGame(Dictionary<string, byte[]> gameStorage, AvailableGames gameName)
        {
            if (!HasInited)
            {
                throw new Exception("Call 'StartConnection' before calling this method");
            }
            if (HasGameStarted)
            {
                throw new Exception("Cannot start game twice");
            }
            HasGameStarted = true;

            GameName = gameName;

            await _scummHubRpc.RunGameAsync(gameName, gameStorage);
        }

        public async Task EnqueueString(string toSend)
        {
            if (!IsGameRunning)
            {
                throw new Exception("Game Is Not Running. Call 'StartConnection' and 'StartGame' before calling this method, and Make Sure You Haven't Quit");
            }

            await _realTimeDataBusClient.EnqueueStringAsync(toSend);
        }

        public async Task EnqueueControlKey(ControlKeys toSend)
        {
            if (!IsGameRunning)
            {
                throw new Exception("Game Is Not Running. Call 'StartConnection' and 'StartGame' before calling this method, and Make Sure You Haven't Quit");
            }

            await _realTimeDataBusClient.EnqueueControlKeyAsync(toSend);
        }

        public async Task EnqueueMouseMove(int x, int y)
        {
            if (!IsGameRunning)
            {
                throw new Exception("Game Is Not Running. Call 'StartConnection' and 'StartGame' before calling this method, and Make Sure You Haven't Quit");
            }

            await _realTimeDataBusClient.EnqueueMouseMoveAsync(x, y);
        }

        public async Task EnqueueMouseClick(MouseClick mouseClick)
        {
            if (!IsGameRunning)
            {
                throw new Exception("Game Is Not Running. Call 'StartConnection' and 'StartGame' before calling this method, and Make Sure You Haven't Quit");
            }

            await _realTimeDataBusClient.EnqueueMouseClickAsync(mouseClick);
        }

        public async Task Quit()
        {
            try
            {
                if (IsGameRunning)
                {
                    await _fireQuitEventSemaphore.WaitAsync();

                    if (IsGameRunning)
                    {
                        //await _proxy.Invoke("Quit"); TODO: Rpc
                        await FireQuitEvent();
                        HasFrontEndQuit = true;
                    }
                }

            }
            finally
            {
                _fireQuitEventSemaphore.Release();
            }
        }
        private async Task FireQuitEvent()
        {
            if (_onQuit != null)
            {
                await _onQuit.Invoke();
            }
        }

        public void SetNextFrameFunctionPointer(CopyRectToScreenAsync screenDrawingCallback)
        {
            _screenDrawingCallback = screenDrawingCallback;
            _realTimeDataEndpoint.OnFrameReceived(screenDrawingCallback, RpcPort);
        }

        public void SetPlaySoundFunctionPointer(PlayAudioAsync playAudio)
        {
            _realTimeDataEndpoint.OnAudioReceived(playAudio, RpcPort);
        }

        public void SetSaveGameFunctionPointer(SaveDataAsync saveDataCallback)
        {
            _saveDataCallback = saveDataCallback;
        }

        public async Task<bool> SaveGameAsync(byte[] saveData, String fileName)
        {
            return await _saveDataCallback?.Invoke(saveData, fileName);
        }

        public async Task SendWholeScreenToNextFrameCallback()
        {
            if (_screenDrawingCallback == null)
            {
                throw new Exception("There needs to be a callback set first");
            }

            await _screenDrawingCallback(await _realTimeDataBusClient.ScheduleRedrawWholeScreen());
        }

        public void BackEndQuit() //TODO: Not ideal, when doing IceAsync Fix
        {
            TaskFactory tf = new TaskFactory();
            tf.StartNew(BackEndQuitAsync);
        }

        public async Task BackEndQuitAsync()
        {
            try
            {
                if (IsGameRunning)
                {
                    await _fireQuitEventSemaphore.WaitAsync();
                    if (IsGameRunning)
                    {
                        HasBackEndQuit = true;
                        await FireQuitEvent();
                    }
                }
            }
            finally
            {
                _fireQuitEventSemaphore.Release();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _realTimeDataEndpoint.Dispose();
                    _scummHubRpc.Dispose();
                    //_rpcPortGetter.Dispose();
                    //_realTimePortGetter.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        public async Task BeforeDispose()
        {
            if (IsGameRunning)
            {
                await Quit();
            }
        }

        public async Task StartSound()
        {
            await _realTimeDataBusClient.StartSoundAsync();
        }

        public async Task StopSound()
        {
            await _realTimeDataBusClient.StopSoundAsync();
        }

        #endregion
    }
}
