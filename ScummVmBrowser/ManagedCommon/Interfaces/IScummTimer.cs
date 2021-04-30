using ManagedCommon.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
	public interface IScummTimer
	{
		bool InstallTimerProc(ScummTimerCallback proc, int intervalMicroseconds, IntPtr refCon, string id);
		void RemoveTimerProc(ScummTimerCallback proc);
	}
}
