using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ManagedCommon.ExtensionMethods;
using SharedMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ManagedCommon.Enums.Actions;
using System.Drawing;
using ManagedCommon.Base;

namespace ScummVMBrowser.Utilities
{
    public class SharedMemoryRealTimeDataBusClient : SharedMemoryBase, IRealTimeDataBusClient
    {
        private RpcBuffer _startSoundMasterBuffer;
        private RpcBuffer _stopSoundMasterBuffer;
        private RpcBuffer _enqueueStringMasterBuffer;
        private RpcBuffer _enqueueControlKeyMasterBuffer;
        private RpcBuffer _enqueueMouseMoveMasterBuffer;
        private RpcBuffer _enqueueMouseClickMasterBuffer;
        private RpcBuffer _getWholeScreenBufferBuffer;

        public SharedMemoryRealTimeDataBusClient(IConfigurationStore<System.Enum> configurationStore): base(configurationStore)
        {

        }

        public async override Task Init(string id)
        {
           await base.Init(id);
            _startSoundMasterBuffer = new RpcBuffer(GetId(RpcBufferNames.StartSound), ConfigurationStore.GetValue<int>(RpcBufferNames.StartSound));
            _stopSoundMasterBuffer = new RpcBuffer(GetId(RpcBufferNames.StopSound), ConfigurationStore.GetValue<int>(RpcBufferNames.StopSound));
            _enqueueStringMasterBuffer = new RpcBuffer(GetId(RpcBufferNames.EnqueueString), ConfigurationStore.GetValue<int>(RpcBufferNames.EnqueueString));
            _enqueueControlKeyMasterBuffer = new RpcBuffer(GetId(RpcBufferNames.EnqueueControlKey), ConfigurationStore.GetValue<int>(RpcBufferNames.EnqueueControlKey));
            _enqueueMouseMoveMasterBuffer = new RpcBuffer(GetId(RpcBufferNames.EnqueueMouseMove), ConfigurationStore.GetValue<int>(RpcBufferNames.EnqueueMouseMove));
            _enqueueMouseClickMasterBuffer = new RpcBuffer(GetId(RpcBufferNames.EnqueueMouseClick), ConfigurationStore.GetValue<int>(RpcBufferNames.EnqueueMouseClick));
            _getWholeScreenBufferBuffer = new RpcBuffer(GetId(RpcBufferNames.GetWholeScreenBuffer), ConfigurationStore.GetValue<int>(RpcBufferNames.GetWholeScreenBuffer));
        }

        public Task StartSoundAsync()
        {
            return _startSoundMasterBuffer.RemoteRequestAsync();
        }

        public Task StopSoundAsync()
        {
            return _stopSoundMasterBuffer.RemoteRequestAsync();
        }

        public Task EnqueueStringAsync(string toSend)
        {
            return _enqueueStringMasterBuffer.RemoteRequestAsync(toSend.ToBinary());
        }

        public Task EnqueueControlKeyAsync(ManagedCommon.Enums.ControlKeys toSend)
        {
            return _enqueueControlKeyMasterBuffer.RemoteRequestAsync(toSend.ToBinary());
        }

        public Task EnqueueMouseMoveAsync(int x, int y)
        {
            return _enqueueMouseMoveMasterBuffer.RemoteRequestAsync(new Point(x, y).ToBinary());
        }

        public Task EnqueueMouseClickAsync(MouseClick mouseClick)
        {
            return _enqueueMouseClickMasterBuffer.RemoteRequestAsync(mouseClick.ToBinary());
        }

        public async Task<List<ScreenBuffer>> ScheduleRedrawWholeScreen()
        {
          RpcResponse response = await _getWholeScreenBufferBuffer.RemoteRequestAsync();
          
            return response.Data.FromBinary<List<ScreenBuffer>>();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _startSoundMasterBuffer.Dispose();
                    _stopSoundMasterBuffer.Dispose();
                    _enqueueStringMasterBuffer.Dispose();
                    _enqueueControlKeyMasterBuffer.Dispose();
                    _enqueueMouseMoveMasterBuffer.Dispose();
                    _enqueueMouseClickMasterBuffer.Dispose();
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

        public Task StartConnectionAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
