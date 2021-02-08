using System;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;
using CLIScumm;
using System.Drawing;
using System.Threading.Tasks;
using CliScummEvents;
using Unity;
using Unity.Lifetime;
using Unity.Injection;
using System.Collections.Generic;
using SignalRSelfHost;
using ManagedCommon.Interfaces;
using ManagedCommon.Enums;
using ConfigStore;
using ManagedCommon.Delegates;
using System.Threading;
using System.Linq;
using NAudio.Wave;
using System.IO;
using ManagedCommon.Enums.Actions;
using ManagedCommon.Interfaces.Rpc;
using Microsoft.VisualStudio.Threading;
using ManagedCommon;
using StartInstance;
using PortSharer;
using System.Diagnostics;
using ManagedCommon.Enums.Logging;
using ManagedCommon.MessageBuffering;

namespace SignalRSelfHost
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
    public class CliScumm : IScummHubRpc
    {
        const short QuitTimeoutMs = 10000;
        private static IWrapper _wrapper;
        private static Task _runningGameTask;
        private Quit _onQuit;
        private object _quitEventLock;
        private IConfigurationStore<Enum> _configurationStore;
        private Timer _killProcessOnQuitTimer;
        private IRealTimeDataBusServer _realTimeDataBus;
        private IScummHubClientRpcProxy _scummVMHubClient;
        private IRealTimeDataEndpointServer _realTimeDataEndpointServer;
		private ProcessMessageBuffers _processMessageBuffers;
        private ILogger _logger;

        public event Quit OnQuit
        {
            add
            {
                lock (_quitEventLock)
                {
                    _onQuit += value;
                }
            }
            remove
            {
                lock (_quitEventLock)
                {
                    _onQuit -= value;
                }
            }
        }


        public CliScumm(IWrapper wrapper, IConfigurationStore<Enum> configurationStore, IRealTimeDataBusServer realTimeDataBus, IScummHubClientRpcProxy scummVMHubClient, IRealTimeDataEndpointServer realTimeDataEndpointServer, ILogger logger, IByteEncoder byteEncoder)
        {
            _wrapper = wrapper;
            _configurationStore = configurationStore;
            _quitEventLock = new object();
            _killProcessOnQuitTimer = new Timer((object state) => WebServer.GameHasEnded = true, null, Timeout.Infinite, Timeout.Infinite);
            _realTimeDataBus = realTimeDataBus;
            _scummVMHubClient = scummVMHubClient;
            _realTimeDataEndpointServer = realTimeDataEndpointServer;
            _logger = logger;
			_processMessageBuffers = new ProcessMessageBuffers(async i => await _realTimeDataBus.DisplayFrameAsync(i), configurationStore, byteEncoder);

		}

        public async Task Init(string rpcPortGetterId, string realTimePortGetterId)
        {

            _scummVMHubClient.Init(_configurationStore.GetValue(ScummConnectionSettings.CliScummHubName), _configurationStore.GetValue(IceRemoteProcBackEnd.ClientName), _configurationStore.GetValue(IceRemoteProcBackEnd.HostName), rpcPortGetterId, this, Protocol.@default);
            await _realTimeDataEndpointServer.Init(realTimePortGetterId);
            _realTimeDataEndpointServer.OnStartSound(StartSound);
            _realTimeDataEndpointServer.OnStopSound(StopSound);
            _realTimeDataEndpointServer.OnEnqueueControlKey(EnqueueControlKey);
            _realTimeDataEndpointServer.OnEnqueueString(EnqueueString);
            _realTimeDataEndpointServer.OnEnqueueMouseClick(EnqueueMouseClick);
            _realTimeDataEndpointServer.OnEnqueueMouseMove(EnqueueMouseMove);
            _realTimeDataEndpointServer.OnEnqueueControlKey(EnqueueControlKey);
            _realTimeDataEndpointServer.OnScheduleRedrawWholeScreen(ScheduleRedrawWholeScreen);
        }


        public Task RunGameAsync(AvailableGames game, Dictionary<string, byte[]> gameSaveData) //Doesn't need to be async but is forced to do so because it implements the same interface as the client. TODO: Fix
        {
            _wrapper.OnCopyRectToScreen += (List<ScreenBuffer> screenBuffers) =>
            {
				_processMessageBuffers.Enqueue(new Message<List<ScreenBuffer>> { MessageType = MessageType.Frames, MessageContents = screenBuffers });
            };
            _wrapper.OnSaveData += (byte[] saveData, String fileName) => _scummVMHubClient.SaveGame(saveData, fileName);

            OnQuit += () =>
            {
                _scummVMHubClient.BackEndQuit();
                _killProcessOnQuitTimer.Change(_configurationStore.GetValue<int>(ScummHubSettings.KillProcessOnQuitTimeoutMs), Timeout.Infinite);
                return Task.CompletedTask;
            };

            _runningGameTask = Task.Run(() => StartGameWrapper(game, gameSaveData));


            return Task.CompletedTask;
        }

        private void StartGameWrapper(AvailableGames game, Dictionary<string, byte[]> gameSaveData)
        {
            try
            {
                _wrapper.RunGame(game, null, gameSaveData, PlaySound);
                _onQuit.Invoke();
            }
            catch (System.Exception e)
            {
                _logger.LogMessage(LoggingLevel.Error, LoggingCategory.CliScummSelfHost, ErrorMessage.GeneralErrorCliScumm, e.ToString());
            }
        }

        private void PlaySound(byte[] sound)
        {
			_processMessageBuffers.Enqueue(new Message<IEnumerable<object>> { MessageType = MessageType.Sound, MessageContents = sound.Cast<Object>() });
		}


        public async Task EndGame()
        {
            _wrapper.Quit();
            Task awaitEndTask = Task.Run(
                   async () =>
                    {
                        await _runningGameTask;
						await _processMessageBuffers.Stop();
                    });
            Task.WaitAny(awaitEndTask, Task.Delay(QuitTimeoutMs));
        }

        public void Quit()
        {
            EndGame().Wait(); //TODO: Fix
        }

        public void EnqueueControlKey(ManagedCommon.Enums.ControlKeys controlKey)
        {
            EnqueueGameEvent(new SendControlCharacters(controlKey));
        }

        public void EnqueueString(String toSend)
        {
            EnqueueGameEvent(new SendString(toSend));
        }

        public void EnqueueMouseMove(int x, int y)
        {
            EnqueueGameEvent(new SendMouseMove(x, y));
        }

        public void EnqueueMouseClick(MouseClick mouseButton)
        {
            EnqueueGameEvent(new SendMouseClick(mouseButton, () => _wrapper.GetCurrentMousePosition()));
        }

        private void EnqueueGameEvent(IGameEvent gameEvent)
        {
            _wrapper.EnqueueGameEvent(gameEvent);
        }

        public async Task WaitForQuit()
        {
            if (_runningGameTask != null)
            {
                await _runningGameTask;
            }
        }

        public void ScheduleRedrawWholeScreen()
        {
            _wrapper.ScheduleRedrawWholeScreen();
        }

        public void StartSound()
        {
            _wrapper.StartSound();
        }

        public void StopSound()
        {
            _wrapper.StopSound();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _scummVMHubClient.Dispose();
                    _realTimeDataEndpointServer.Dispose();
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
        #endregion
    }
}
