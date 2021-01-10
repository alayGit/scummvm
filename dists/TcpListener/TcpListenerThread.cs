using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
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
		private Action<byte[]> _onMessage;
		private bool _stopClientTask;
		private int _sleepTime;
		private AsyncQueue<byte[]> _toSendQueue;
		private AsyncSemaphore _asyncSemaphore;

		public static readonly string Terminator = ((char)0).ToString();
		const int BufferSize = 500;


		public TcpListenerThread(Action<byte[]> onMessage, int sleepTime)
		{
			_stopClientTask = false;
			_onMessage = onMessage;
			_sleepTime = sleepTime;
			_runTcpListenerTask = Task.Run(async () => await RunTcpListenerTask());
			_asyncSemaphore = new AsyncSemaphore(1);
		}

		protected abstract Task<NetworkStream> ClientStream();

		private async Task RunTcpListenerTask()
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
								_onMessage(streamData.ToArray());
								streamData.Clear();
							}
						}
					}
				}
			}
		}

		internal async Task Connect()
		{
			await ClientStream();
		}

		internal async Task SendObject(byte[] objectToSend)
		{
			NetworkStream networkStream = await ClientStream();

			using (await _asyncSemaphore.EnterAsync())
			{
				networkStream.ReadTimeout = 2000;

				using (StreamWriter writer = new StreamWriter(networkStream))
				{
					List<byte> terminatedBytesToSend = objectToSend.ToList();
					terminatedBytesToSend.Add(0);

					networkStream.Write(terminatedBytesToSend.ToArray(), 0, terminatedBytesToSend.Count());
				}
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

