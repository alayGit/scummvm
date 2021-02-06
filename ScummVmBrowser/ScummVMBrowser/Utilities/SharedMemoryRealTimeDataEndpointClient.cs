using ManagedCommon.Base;
using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.ExtensionMethods;
using ManagedCommon.Interfaces;
using SharedMemory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ScummVMBrowser.Utilities
{
    public class SharedMemoryRealTimeDataEndpointClient : SharedMemoryBase, IRealTimeDataEndpointClient
    {
        private CopyRectToScreenAsync _copyRectToScreenCallback;
        private RpcBuffer _copyRectToScreenSlaveBuffer;
        private PlayAudioAsync _playSoundCallback;
        private RpcBuffer _playSoundSlaveBuffer;

        public SharedMemoryRealTimeDataEndpointClient(IConfigurationStore<System.Enum> configurationStore) : base(configurationStore)
        {
        }

        public void OnAudioReceived(PlayAudioAsync playAudio, int instanceId)
        {
            _playSoundCallback = playAudio;

            if (_playSoundSlaveBuffer == null)
            {
                _playSoundSlaveBuffer = new RpcBuffer(GetId(RpcBufferNames.PlaySound), async (msgId, payLoad) =>
                {
                   await _playSoundCallback?.Invoke(payLoad.FromBinary<byte[]>());
                });
            }
        }

        public void OnFrameReceived(CopyRectToScreenAsync copyRectToScreen, int instanceId)
        {
            _copyRectToScreenCallback = copyRectToScreen;

            if (_copyRectToScreenSlaveBuffer == null)
            {
                _copyRectToScreenSlaveBuffer = new RpcBuffer(GetId(RpcBufferNames.DisplayFrame), (msgId, payLoad) =>
                {
                    _copyRectToScreenCallback?.Invoke(payLoad.FromBinary<List<List<ScreenBuffer>>>());
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
                    _playSoundSlaveBuffer.Dispose();
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
