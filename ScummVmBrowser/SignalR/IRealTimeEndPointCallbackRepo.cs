using ManagedCommon.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR
{
   public interface IRealTimeEndPointCallbackRepo
    {
 		EnqueueInputMessages EnqueueInputMessages { get; }
		StartSound StartSound { get; }
        StopSound StopSound { get; }
        ScheduleRedrawWholeScreen ScheduleRedrawWholeScreen { get; }
    }
}
