using ManagedCommon.Base;
using ManagedCommon.Delegates;
using ManagedCommon.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	public class TcpListenerRealTimeDataEndpointServer : IRealTimeDataEndpointServer
	{
		private IStarter _starter;
		private IConfigurationStore<System.Enum> _configurationStore;
		private TcpClientListenerThread _tcpClientListenerThread;

		public TcpListenerRealTimeDataEndpointServer(IStarter starter, IConfigurationStore<System.Enum> configurationStore)
		{
			_starter = starter;
			_configurationStore = configurationStore;
		}


		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public async Task Init(string port)
		{
			_tcpClientListenerThread = new TcpClientListenerThread(OnMessage, int.Parse(port), 5);

			await _tcpClientListenerThread.Connect();
		}

		public void OnEnqueueControlKey(EnqueueControlKey enqueueControlKey)
		{
			throw new NotImplementedException();
		}

		public void OnEnqueueMouseClick(EnqueueMouseClick enqueueMouseClick)
		{
			throw new NotImplementedException();
		}

		public void OnEnqueueMouseMove(EnqueueMouseMove enqueueMouseMove)
		{
			throw new NotImplementedException();
		}

		public void OnEnqueueString(EnqueueString enqueueString)
		{
			throw new NotImplementedException();
		}

		public void OnScheduleRedrawWholeScreen(ScheduleRedrawWholeScreen getWholeScreenBuffer)
		{
			throw new NotImplementedException();
		}

		public void OnStartSound(StartSound startSound)
		{
			throw new NotImplementedException();
		}

		public void OnStopSound(StopSound stopSound)
		{
			throw new NotImplementedException();
		}


	   public void OnMessage(byte[] value)
		{
			string json = Encoding.ASCII.GetString(value);
			List<ScreenBuffer> b = JsonConvert.DeserializeObject<List<ScreenBuffer>>(json);
		}
	}
}
