using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using ManagedCommon.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
   public interface IRealTimeDataEndpointServer: IDisposable
    {                                                                                              
		void OnEnqueueInputMessages(EnqueueInputMessages enqueueInputMessages);
		void OnScheduleRedrawWholeScreen(ScheduleRedrawWholeScreen getWholeScreenBuffer);
		void OnStartSound(StartSound startSound);
        void OnStopSound(StopSound stopSound);
		Task Init(string id);
	}
}
