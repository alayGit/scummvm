using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	internal abstract class TcpListenerThread
	{
		private Task _runTcpListenerTask;
		private bool _disposedValue;
		private bool _stopClientTask;
		private int _sleepTime;
		private AsyncSemaphore _asyncSemaphore;

		public static readonly string Terminator = ((char)0).ToString();
		const int BufferSize = 500;


		public TcpListenerThread()
		{
			_stopClientTask = false;
			_asyncSemaphore = new AsyncSemaphore(1);
		}

		protected abstract Task<NetworkStream> ClientStream();

		internal void RunTcpListenerTask(Func<byte[], Task> onMessage, int sleepTime)
		{
			_runTcpListenerTask = Task.Run(async () =>
			{
				while (!_stopClientTask)
				{
					NetworkStream stream = await ClientStream();

					using (await _asyncSemaphore.EnterAsync())
					{
						List<byte> streamData = new List<byte>();
						while (stream.DataAvailable)
						{
							byte[] buffer = new byte[BufferSize];
							int dataCount = stream.Read(buffer, 0, BufferSize);

							IEnumerable<byte> readBuffer = buffer.Take(dataCount);

							foreach (byte b in readBuffer)
							{
								if (b != 0)
								{
									streamData.Add(b);
								}
								else
								{
									await onMessage(streamData.ToArray());
									streamData.Clear();
								}
							}
						}
					}
					await Task.Delay(sleepTime);
				}
			}
			);
		}

		internal async Task Connect()
		{
			await ClientStream();
		}

		internal async Task SendAsciiBytes(byte[] objectToSend)
		{
			using (await _asyncSemaphore.EnterAsync())
			{
				(await ClientStream()).ReadTimeout = 2000;

				List<byte> terminatedBytesToSend = objectToSend.ToList();
				terminatedBytesToSend.Add(0);

				(await ClientStream()).Write(terminatedBytesToSend.ToArray(), 0, terminatedBytesToSend.Count());


			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_stopClientTask = true;
					_runTcpListenerTask.Wait();
				}

				_disposedValue = true;
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

