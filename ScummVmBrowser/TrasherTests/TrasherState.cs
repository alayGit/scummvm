using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrasherTests
{
    public class TrasherState: IDisposable
    {
       public MockITrashable Trashable { get; set; }
       public List<TrashResult> TrashResults { get; set; }
       public Trasher.Trasher Trasher { get; set; }

        public TrasherState()
        {
            TrashResults = new List<TrashResult>() { new TrashResult(), new TrashResult(), new TrashResult() };
            Trashable = new MockITrashable();
            Trasher = new Trasher.Trasher(Trashable, 1, TrashResults[TrashResults.Count - 1].OnTrashed);

            Trashable.SubscribeToTrashEvent(TrashResults[0].OnTrashedAsync);
            Trashable.SubscribeToTrashEvent(TrashResults[1].OnTrashedAsync);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Trasher.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
