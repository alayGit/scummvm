using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ManagedCommon.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ManagedCommon.Enums.Actions;
using System.Drawing;
using ManagedCommon.Base;
using ManagedCommon.Delegates;
using SignalRHostWithUnity;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Client;
using ManagedCommon.Enums.Settings;

namespace ScummVMBrowser.Utilities
{
    public class SignalRRealTimDataBusAndEndpointClient : SharedMemoryBase, IRealTimeDataBusClient, IRealTimeDataEndpointClient
    {
        private IHubProxyConnection _connection;
        private IHubConnectionFactory _hubConnectionFactory;
        private IConfigurationStore<System.Enum> _configurationStore;
        private IHubProxy _proxy;
        private CopyRectToScreenAsync _copyRectToScreenCallback;
        private PlayAudioAsync _playSoundCallback;
        private IDisposable _onNextFrameDisposeHandler;
        private IDisposable _onPlayAudioDisposeHandler;
        private bool _hasInited; 

        public SignalRRealTimDataBusAndEndpointClient(IConfigurationStore<System.Enum> configurationStore, IHubConnectionFactory hubConnectionFactory) : base(configurationStore)
        {
            _configurationStore = configurationStore;
            _hubConnectionFactory = hubConnectionFactory;
    //TODO Pass in port and put url in config
            _hasInited = false;
        }

        public async override Task Init(string id)
        {
            if (!_hasInited) //As this class is both bus and endpoint init will be likely called twice by the client
            { 
                await base.Init(id);
                _connection = _hubConnectionFactory.CreateConnection($"http://{_configurationStore.GetValue(ScummConnectionSettings.CliScummHubHostName)}:{id}");
                _proxy = _connection.CreateHubProxy(_configurationStore.GetValue(ScummConnectionSettings.CliScummHubName));
                _hasInited = true;
            }
        }

        public async Task StartConnectionAsync()
        {
            await _connection.Start();
        }

        public async Task StartSoundAsync()
        {
            await _proxy.Invoke("StartSound");
        }

        public async Task StopSoundAsync()
        {
            await _proxy.Invoke("StopSound");
        }

        public async Task EnqueueStringAsync(string toSend)
        {
            await _proxy.Invoke("EnqueueString", toSend);
        }

        public async Task EnqueueControlKeyAsync(ManagedCommon.Enums.ControlKeys toSend)
        {
           await _proxy.Invoke("EnqueueControlKey", toSend);
        }

        public async Task EnqueueMouseMoveAsync(int x, int y)
        {
            await _proxy.Invoke("EnqueueMouseMove", x, y);
        }

        public async Task EnqueueMouseClickAsync(MouseClick mouseClick)
        {
            await _proxy.Invoke("EnqueueMouseClick", mouseClick);
        }

        public async Task<List<KeyValuePair<MessageType, string>>> ScheduleRedrawWholeScreen()
        {
          return await _proxy.Invoke<List<KeyValuePair<MessageType, string>>>("ScheduleRedrawWholeScreen");
        }

        public void OnAudioReceived(PlayAudioAsync playAudio, int instanceId)
        {
            _playSoundCallback = playAudio;

            if (_onPlayAudioDisposeHandler == null)
            {
                _onPlayAudioDisposeHandler = _proxy.On("PlayAudio", (byte[] data) =>
                {
                    _playSoundCallback?.Invoke(data);
                });
            }
        }

        public void OnFrameReceived(CopyRectToScreenAsync copyRectToScreen, int instanceId)
        {
            _copyRectToScreenCallback = copyRectToScreen;

            if (_onNextFrameDisposeHandler == null)
            {
                _onNextFrameDisposeHandler = _proxy.On("NextFrame", (List<KeyValuePair<MessageType, string>> screenBuffers) =>
                {
                    _copyRectToScreenCallback?.Invoke(screenBuffers);
                });
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
                    _connection.Stop();
                    _connection.Dispose();
                    _onPlayAudioDisposeHandler?.Dispose();
                    _onNextFrameDisposeHandler?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
