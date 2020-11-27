using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
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
        void OnEnqueueString(EnqueueString enqueueString);
        void OnEnqueueControlKey(EnqueueControlKey enqueueControlKey);
        void OnEnqueueMouseMove(EnqueueMouseMove enqueueMouseMove);
        void OnEnqueueMouseClick(EnqueueMouseClick enqueueMouseClick);
		void OnGetRedrawWholeScreenBuffersCompressed(GetRedrawWholeScreenBuffersCompressed getWholeScreenBuffer);
		void OnStartSound(StartSound startSound);
        void OnStopSound(StopSound stopSound);
		Task Init(string id);
	}
}
