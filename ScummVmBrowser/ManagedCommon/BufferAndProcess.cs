using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManagedCommon
{
	public class BufferAndProcess<T>
	{
		public bool Stopped { get; set; }


		private Task _processTask;
		private Func<IEnumerable<T>, Task> _processCallback;
		private List<T> _dataList;
		private AsyncSemaphore _listLock;
		private Task _queueTask;

		public BufferAndProcess(Func<IEnumerable<T>, Task> processCallback, IConfigurationStore<Enum> configurationStore)
		{
			Stopped = false;
			_processCallback = processCallback;
			_dataList = new List<T>();
			_listLock = new AsyncSemaphore(1);

			_processTask = Task.Run(async () =>
			{
				List<T> dataListCopy = null;
				while (!Stopped)
				{
					using (await _listLock.EnterAsync())
					{
						if (_dataList.Count() != 0)
						{
							if (_dataList.Count > 1)
							{
								Debug.WriteLine("Process Queue " + _dataList.Count);
							}
							dataListCopy = _dataList.ToList(); //Copy the list so that we are not holding onto the lock during network call
							_dataList = new List<T>();

							await _processCallback(dataListCopy);
						}
					}
					await Task.Delay(configurationStore.GetValue<int>(ScummHubSettings.BufferAndProcessSleepTime));
				}
			});
		}

		public void Enqueue(IEnumerable<T> data)
		{
			if (_queueTask == null)
			{
				_queueTask = StartQueueTask(data);
			}
			else
			{
				_queueTask = _queueTask.ContinueWith(t => StartQueueTask(data));
			}

		}

		async Task StartQueueTask(IEnumerable<T> data)
		{
			using (await _listLock.EnterAsync())
			{
				_dataList.AddRange(data);
			}
		}

		public async Task Stop()
		{
			if (Stopped)
			{
				throw new Exception("Already stopped");
			}

			Stopped = true;

			await _processTask;
		}
	}
}
