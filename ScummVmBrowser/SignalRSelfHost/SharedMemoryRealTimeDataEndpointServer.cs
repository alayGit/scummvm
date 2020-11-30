using ManagedCommon.Base;
using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using ManagedCommon.ExtensionMethods;
using ManagedCommon.Interfaces;
using SharedMemory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ScummVMBrowser.Utilities
{
    public class SharedMemoryRealTimeDataEndpointServer : SharedMemoryBase, IRealTimeDataEndpointServer
    {
        private EnqueueString _enqueueStringCallback;
        private RpcBuffer _enqueueStringSlaveBuffer;
        private EnqueueControlKey _enqueueControlKey;
        private RpcBuffer _enqueueControlKeySlaveBuffer;
        private EnqueueMouseMove _enqueueMouseMove;
        private RpcBuffer _enqueueMouseMoveSlaveBuffer;
        private EnqueueMouseClick _enqueueMouseClick;
        private RpcBuffer _enqueueMouseClickSlaveBuffer;
        private StartSound _startSound;
        private RpcBuffer _startSoundSlaveBuffer;
        private StopSound _stopSound;
        private RpcBuffer _stopSoundSlaveBuffer;
        private RpcBuffer _getWholeScreenSlaveBuffer;
        private ScheduleRedrawWholeScreen _getWholeScreenBuffer;

        public SharedMemoryRealTimeDataEndpointServer(IConfigurationStore<System.Enum> configurationStore) : base(configurationStore)
        {
        }

        public void OnEnqueueString(EnqueueString enqueueString)
        {
            _enqueueStringCallback = enqueueString;

            if (_enqueueStringSlaveBuffer == null)
            {
                _enqueueStringSlaveBuffer = new RpcBuffer(GetId(RpcBufferNames.EnqueueString), (msgId, payLoad) =>
                {
                    _enqueueStringCallback?.Invoke(payLoad.FromBinary<string>());
                });
            }
        }

        public void OnEnqueueControlKey(EnqueueControlKey enqueueControlKey)
        {
            _enqueueControlKey = enqueueControlKey;

            if (_enqueueControlKeySlaveBuffer == null)
            {
                _enqueueControlKeySlaveBuffer = new RpcBuffer(GetId(RpcBufferNames.EnqueueControlKey), (msgId, payLoad) =>
                {
                    _enqueueControlKey?.Invoke(payLoad.FromBinary<ManagedCommon.Enums.ControlKeys>()); //TODO: Delete the other control keys
                });
            }
        }

        public void OnEnqueueMouseMove(EnqueueMouseMove enqueueMouseMove)
        {
            _enqueueMouseMove = enqueueMouseMove;

            if (_enqueueMouseMoveSlaveBuffer == null)
            {
                _enqueueMouseMoveSlaveBuffer = new RpcBuffer(GetId(RpcBufferNames.EnqueueMouseMove), (msgId, payLoad) =>
                {
                    Point point = payLoad.FromBinary<Point>();
                    _enqueueMouseMove?.Invoke(point.X, point.Y);
                });
            }
        }

        public void OnEnqueueMouseClick(EnqueueMouseClick enqueueMouseClick)
        {
            _enqueueMouseClick = enqueueMouseClick;

            if (_enqueueMouseClickSlaveBuffer == null)
            {
                _enqueueMouseClickSlaveBuffer = new RpcBuffer(GetId(RpcBufferNames.EnqueueMouseClick), (msgId, payLoad) =>
                {
                    _enqueueMouseClick?.Invoke(payLoad.FromBinary<MouseClick>());
                });
            }
        }

        public void OnStartSound(StartSound startSound)
        {
            _startSound = startSound;

            if (_startSoundSlaveBuffer == null)
            {
                _startSoundSlaveBuffer = new RpcBuffer(GetId(RpcBufferNames.StartSound), (msgId, payLoad) =>
                {
                    _startSound?.Invoke();
                });
            }
        }

        public void OnStopSound(StopSound stopSound)
        {
            _stopSound = stopSound;

            if (_stopSoundSlaveBuffer == null)
            {
                _stopSoundSlaveBuffer = new RpcBuffer(GetId(RpcBufferNames.StopSound), (msgId, payLoad) =>
                {
                    _stopSound?.Invoke();

                });
            }
        }

        public void OnScheduleRedrawWholeScreen(ScheduleRedrawWholeScreen getWholeScreenBuffer)
        {
            _getWholeScreenBuffer = getWholeScreenBuffer;

            if (_getWholeScreenSlaveBuffer == null)
            {
                _getWholeScreenSlaveBuffer = new RpcBuffer(GetId(RpcBufferNames.GetWholeScreenBuffer), (msgId, payLoad) => _getWholeScreenBuffer?.Invoke());
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
                    _enqueueStringSlaveBuffer?.Dispose();
                    _enqueueControlKeySlaveBuffer?.Dispose();
                    _enqueueMouseClickSlaveBuffer?.Dispose();
                    _startSoundSlaveBuffer?.Dispose();
                    _stopSoundSlaveBuffer?.Dispose();
                    _getWholeScreenSlaveBuffer?.Dispose();
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
