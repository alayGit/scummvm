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
        EnqueueString EnqueueStringCallback { get; }
        EnqueueControlKey EnqueueControlKey { get; }
        EnqueueMouseMove EnqueueMouseMove { get; }
        EnqueueString EnqueueString { get; }
        EnqueueMouseClick EnqueueMouseClick { get; }
		EnqueueInputMessages EnqueueInputMessages { get; }
		StartSound StartSound { get; }
        StopSound StopSound { get; }
        ScheduleRedrawWholeScreen ScheduleRedrawWholeScreen { get; }
    }
}
