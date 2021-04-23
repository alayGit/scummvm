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
using ManagedCommon.Models;
using ManagedCommon.Enums.Other;
using ManagedCommon.Constants;

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
		private IProcessMessageBuffers _processMessageBuffers;
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


        public CliScumm(IWrapper wrapper, IConfigurationStore<Enum> configurationStore, IRealTimeDataBusServer realTimeDataBus, IScummHubClientRpcProxy scummVMHubClient, IRealTimeDataEndpointServer realTimeDataEndpointServer, ILogger logger, IByteEncoder byteEncoder, IProcessMessageBuffers processMessageBuffers)
        {
            _wrapper = wrapper;
            _configurationStore = configurationStore;
            _quitEventLock = new object();
            _killProcessOnQuitTimer = new Timer((object state) => WebServer.GameHasEnded = true, null, Timeout.Infinite, Timeout.Infinite);
            _realTimeDataBus = realTimeDataBus;
            _scummVMHubClient = scummVMHubClient;
            _realTimeDataEndpointServer = realTimeDataEndpointServer;
            _logger = logger;
			_processMessageBuffers = processMessageBuffers;
			_processMessageBuffers.MessagesProcessed = async gameMessage => await _realTimeDataBus.SendGameMessagesAsync(gameMessage);
		}

        public async Task Init(string rpcPortGetterId, string realTimePortGetterId)
        {

            _scummVMHubClient.Init(_configurationStore.GetValue(ScummConnectionSettings.CliScummHubName), _configurationStore.GetValue(IceRemoteProcBackEnd.ClientName), _configurationStore.GetValue(IceRemoteProcBackEnd.HostName), rpcPortGetterId, this, Protocol.@default);
            await _realTimeDataEndpointServer.Init(realTimePortGetterId);
            _realTimeDataEndpointServer.OnStartSound(StartSound);
            _realTimeDataEndpointServer.OnStopSound(StopSound);
            _realTimeDataEndpointServer.OnScheduleRedrawWholeScreen(ScheduleRedrawWholeScreen);
			_realTimeDataEndpointServer.OnEnqueueInputMessages(EnqueueInputMessages);
        }


        public Task RunGameAsync(AvailableGames game, string compressedAndEncodedGameSaveData) //Doesn't need to be async but is forced to do so because it implements the same interface as the client. TODO: Fix
        {
            _wrapper.SendScreenBuffers += (List<ScreenBuffer> screenBuffers) =>
            {
				_processMessageBuffers.Enqueue(screenBuffers, MessageType.Frames);
            };
            _wrapper.OnSaveData += (string gameSaves) => _scummVMHubClient.SaveGame(gameSaves);

            OnQuit += () =>
            {
                _scummVMHubClient.BackEndQuit();
                _killProcessOnQuitTimer.Change(_configurationStore.GetValue<int>(ScummHubSettings.KillProcessOnQuitTimeoutMs), Timeout.Infinite);
                return Task.CompletedTask;
            };

            _runningGameTask = Task.Run(() => StartGameWrapper(game, compressedAndEncodedGameSaveData));


            return Task.CompletedTask;
        }

        private void StartGameWrapper(AvailableGames game, string compressedAndEncodedGameSaveData)
        {
            try
            {
				_wrapper.RunGame(game, null, compressedAndEncodedGameSaveData, PlaySound, Constants.DoNotLoadSaveSlot);
                _onQuit.Invoke();
            }
            catch (System.Exception e)
            {
                _logger.LogMessage(LoggingLevel.Error, LoggingCategory.CliScummSelfHost, ErrorMessage.GeneralErrorCliScumm, e.ToString());
            }
        }

        private void PlaySound(byte[] sound)
        {
			_processMessageBuffers.Enqueue(sound, MessageType.Sound);
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

		public void EnqueueInputMessages(InputMessage[] inputMessages)
		{
			foreach(InputMessage inputMessage in inputMessages)
			{
				switch(inputMessage.InputType)
				{
					case InputType.ControlKey:
						EnqueueGameEvent(new SendControlCharacters((ControlKeys)Enum.Parse(typeof(ControlKeys), inputMessage.Input)));
						break;
					case InputType.MouseClick:
						EnqueueGameEvent(new SendMouseClick((MouseClick)Enum.Parse(typeof(MouseClick), inputMessage.Input), () => _wrapper.GetCurrentMousePosition()));
						break;
					case InputType.MouseMove:
						string[] xy = inputMessage.Input.Split(',');
						EnqueueGameEvent(new SendMouseMove(int.Parse(xy[0]), int.Parse(xy[1])));
						break;
					case InputType.TextString:
						EnqueueGameEvent(new SendString(inputMessage.Input));
						break;
				}
			}
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
