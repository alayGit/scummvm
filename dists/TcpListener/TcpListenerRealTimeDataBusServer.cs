using ManagedCommon.Base;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using Newtonsoft.Json;
using PortSharer;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	public class TcpListenerRealTimeDataBusServer : IRealTimeDataBusServer
	{

		private IStarter _starter;
		private IConfigurationStore<System.Enum> _configurationStore;
		private IPortSender _portSender;

			public TcpListenerRealTimeDataBusServer(IStarter starter, IPortSender portSender, IConfigurationStore<System.Enum> configurationStore)
		{
			_starter = starter;
			_configurationStore = configurationStore;
			_portSender = portSender;
		}

		public async Task DisplayFrameAsync(List<ScreenBuffer> screenBuffers)
		{
			await Singleton.GetTcpListener(GetPort(), ListenerTypeEnum.Server).SendAsciiBytes(Helpers.ConvertObjectToMessage(screenBuffers, MessageTypeEnum.OnFrameReceived));
		}

		public void Dispose()
		{

		}

		public Task Init(string id)
		{
			return Task.CompletedTask;
		}

		public async Task PlaySound(byte[] data)
		{
			await Singleton.GetTcpListener(GetPort(), ListenerTypeEnum.Server).SendAsciiBytes(Helpers.ConvertObjectToMessage(Convert.ToBase64String(data), MessageTypeEnum.OnPlaySound));
		}

		private int GetPort()
		{
			int port = _portSender.ChosenPort;

			if(port == 0)
			{
				throw new Exception("Fail: Port should have being set by initialization of TcpListenerRealTimeBusServer");
			}

			return port;
		}
	}
}
