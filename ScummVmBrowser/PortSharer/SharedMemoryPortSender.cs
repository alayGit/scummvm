using ManagedCommon.ExtensionMethods;
using ManagedCommon.Interfaces;
using SharedMemory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortSharer
{
    public class SharedMemoryPortSender : IPortSender
    {
        RpcBuffer _rpcBuffer;
        
        public void Init(GetPort getPort, string id)
        {
            _rpcBuffer = new RpcBuffer(id, (msgId, payLoad) =>
            {
				//int port = 0;
				//while (port == 0)
				//{
				//    port = getPort();

				//    if (port == 0)
				//    {
				//        await Task.Delay(5);
				//    }
				//}
                ChosenPort = getPort();

				return ChosenPort.ToBinary();
            });
        }

		public int ChosenPort { get; set; }

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
