using ManagedCommon.ExtensionMethods;
using ManagedCommon.Interfaces;
using SharedMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortSharer
{
    public class SharedMemoryPortGetter : IPortGetter
    {
        RpcBuffer _rpcBuffer;
        public async Task<int> GetPort(string id)
        {
            if(_rpcBuffer == null)
            {
                _rpcBuffer = new RpcBuffer(id);
            }

            return (await _rpcBuffer.RemoteRequestAsync()).Data.FromBinary<int>();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _rpcBuffer?.Dispose();
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
