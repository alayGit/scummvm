using ManagedCommon.Delegates;
using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
namespace ScummTimer
{
	public class ManagedScummTimer : IScummTimer
	{
		public ManagedScummTimer()
		{
			_timers = new Dictionary<ScummTimerCallback, Timer>();
			_syncLock = new object();
		}

		private Dictionary<ScummTimerCallback, Timer> _timers;
		private object _syncLock;

		public bool InstallTimerProc(ScummTimerCallback proc, int interval, IntPtr refCon, string id)
		{
			Timer timer = new Timer(interval);
			timer.Elapsed += (s, e) => proc(refCon);
			timer.Start();

			lock (_syncLock)
			{
				if (_timers.ContainsKey(proc))
				{
					removeTimerProcNoLock(proc);
				}

				_timers.Add(proc, timer);
			}

			return true;
		}

		public void removeTimerProc(ScummTimerCallback proc)
		{
			lock(_syncLock)
			{
				removeTimerProcNoLock(proc);
			}
		}

		private void removeTimerProcNoLock(ScummTimerCallback proc)
		{
			_timers[proc].Stop();
			_timers[proc] = null;
		}
	}
}
