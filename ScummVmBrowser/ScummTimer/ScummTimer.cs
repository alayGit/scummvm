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
	public class ManagedScummTimer : IScummTimer, IDisposable
	{
		public const string AlreadyExistsIdError = "Fail, timer id {0} already exists with a different callback";
		public const string AlreadyExistsTimerError = "Fail, timer callback already exists. The id is {0}";

		public ManagedScummTimer(ILogger logger)
		{
			_timers = new Dictionary<ScummTimerCallback, Timer>();
			_timerIds = new Dictionary<ScummTimerCallback, string>();
			_syncLock = new object();
			_logger = logger;
			disposedValue = false;
		}

		private Dictionary<ScummTimerCallback, Timer> _timers;
		private Dictionary<ScummTimerCallback, string> _timerIds;
		private object _syncLock;
		private ILogger _logger;
		private bool disposedValue;

		public bool InstallTimerProc(ScummTimerCallback proc, int intervalMicroseconds, IntPtr refCon, string id)
		{
			lock (_syncLock)
			{
				if(disposedValue)
				{
					throw new ObjectDisposedException(nameof(ScummTimer));
				}

				ThrowIfDuplicates(proc, id);
				_timerIds.Add(proc, id);

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
				error = String.Format(AlreadyExistsIdError, _timerIds[proc]);
			}
			else if (_timers.ContainsKey(proc))
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

		public void RemoveTimerProc(ScummTimerCallback proc)
		{
			lock(_syncLock)
			{
				if (disposedValue)
				{
					throw new ObjectDisposedException(nameof(ScummTimer));
				}

				if (_timers.ContainsKey(proc))
				{
					_timers[proc].Stop();
					_timers[proc].Dispose();
					_timers.Remove(proc);
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			lock (_syncLock)
			{
				if (!disposedValue)
				{
					if (disposing)
					{
						foreach (ScummTimerCallback kvp in new List<ScummTimerCallback>(_timers.Keys))
						{
							RemoveTimerProc(kvp);
						}
					}

					// TODO: free unmanaged resources (unmanaged objects) and override finalizer
					// TODO: set large fields to null
					disposedValue = true;
				}
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
