using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedCommon.Interfaces;
using Microsoft.VisualStudio.Threading;

namespace Trasher
{
    public class AntiDisposalLock<T> : IAntiDisposalLock<T> where T : IDisposable
    {
        public T Obj { get; private set; }

        private AsyncReaderWriterLock.Releaser? _releaser;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        public AntiDisposalLock(T obj, AsyncReaderWriterLock.Releaser? releaser)
        {
            Obj = obj;
            _releaser = releaser;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                _releaser?.Dispose();
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
