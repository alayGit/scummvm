using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrasherTests
{
    public class TrashResult
    {
        public DateTime TimeWhenCalled { get; private set; }

        public TrashResult()
        {
            TimeWhenCalled = DateTime.MinValue;
        }

        public Task OnTrashedAsync()
        {
            OnTrashed();
            return Task.CompletedTask;
        }

        public void OnTrashed()
        {
            TimeWhenCalled = DateTime.Now;

        }
    }
}
