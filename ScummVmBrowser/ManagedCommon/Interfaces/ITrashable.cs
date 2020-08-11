using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
   public interface ITrashable: IDisposable
    {
        IEnumerable<Func<Task>> WhenTrashedSubscribers { get; }
        void SubscribeToTrashEvent(Func<Task> onTrashed);
    }
}
