using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trasher;

namespace TrasherTests
{
    public class MockITrashable : ITrashable
    {
        public IEnumerable<Func<Task>> WhenTrashedSubscribers => whenTrashedSubscribers;

        private List<Func<Task>> whenTrashedSubscribers;

        public MockITrashable()
        {
            whenTrashedSubscribers = new List<Func<Task>>();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void SubscribeToTrashEvent(Func<Task> onTrashed)
        {
            whenTrashedSubscribers.Add(onTrashed);
        }
    }
}
