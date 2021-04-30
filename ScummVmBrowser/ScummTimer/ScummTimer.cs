using ManagedCommon.Delegates;
using ManagedCommon.Enums.Logging;
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
		public const string AlreadyExistsIdError = "Fail, timer id {0} already exists with a different callback";
		public const string AlreadyExistsTimerError = "Fail, timer callback already exists. The id is {0}";

		public ManagedScummTimer(ILogger logger)
		{
			_timers = new Dictionary<ScummTimerCallback, Timer>();
			_timerIds = new Dictionary<ScummTimerCallback, string>();
			_syncLock = new object();
			_logger = logger;
		}

		private Dictionary<ScummTimerCallback, Timer> _timers;
		private Dictionary<ScummTimerCallback, string> _timerIds;
		private object _syncLock;
		private ILogger _logger;

		public bool InstallTimerProc(ScummTimerCallback proc, int intervalMicroseconds, IntPtr refCon, string id)
		{
			lock (_syncLock)
			{
				ThrowIfDuplicates(proc, id);

				Timer timer = new Timer(intervalMicroseconds / 1000);
				timer.Elapsed += (s, e) => proc(refCon);
				timer.Start();

				_timers.Add(proc, timer);
			}

			return true;
		}

		private string ThrowIfDuplicates(ScummTimerCallback proc, string id)
		{
			string error = null;
			if (_timerIds.ContainsKey(proc) && _timerIds[proc] != id)
			{
				error = String.Format(AlreadyExistsIdError, id);
			}

			if (_timers.ContainsKey(proc))
			{
				error = String.Format(AlreadyExistsTimerError, id);
			}

			if (error != null)
			{
				_logger.LogMessage(LoggingLevel.Error, LoggingCategory.CliScummSelfHost, ErrorMessage.GeneralErrorCliScumm, error);
				throw new Exception(error);
			}

			return error;
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
			if (_timers.ContainsKey(proc))
			{
				_timers[proc].Stop();
				_timers.Remove(proc);
			}
		}
	}
}
