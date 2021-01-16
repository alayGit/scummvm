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
using System.Drawing;
using ManagedCommon.Base;
using System.IO;

namespace SignalRSelfHost
{
    public class SharedMemoryRealTimeDataBusServer : SharedMemoryBase, IRealTimeDataBusServer
    {
        private RpcBuffer _displayFrameMasterBuffer;
        private RpcBuffer _playSoundMasterBuffer;


        public SharedMemoryRealTimeDataBusServer(IConfigurationStore<System.Enum> configurationStore) : base(configurationStore)
        {
        }


        public async override Task Init(string id)
        {
          await base.Init(id);

            _displayFrameMasterBuffer = new RpcBuffer(GetId(RpcBufferNames.DisplayFrame), ConfigurationStore.GetValue<int>(RpcBufferNames.DisplayFrame));
            _playSoundMasterBuffer = new RpcBuffer(GetId(RpcBufferNames.PlaySound), ConfigurationStore.GetValue<int>(RpcBufferNames.PlaySound));
        }

        public async Task PlaySound(byte[] data)
        { 
          await _playSoundMasterBuffer.RemoteRequestAsync(data.ToBinary());
        }

        public async Task DisplayFrameAsync(List<ScreenBuffer> screenBuffers)
        {
            //foreach (ScreenBuffer s in screenBuffers)
            //{
            //    File.AppendAllText("c:\\temp\\no2.txt", s.No.ToString() + "\n");
            //}

            var x = screenBuffers.ToBinary().FromBinary<List<ScreenBuffer>>();
            await _displayFrameMasterBuffer.RemoteRequestAsync(screenBuffers.ToBinary());
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _displayFrameMasterBuffer.Dispose();
                    _playSoundMasterBuffer.Dispose();
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
