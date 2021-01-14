using ManagedCommon.Base;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using Newtonsoft.Json;
using PortSharer;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	public class TcpListenerRealTimeDataBusServer : RealTimeDataBusServer, IRealTimeDataBusServer
	{

		private IStarter _starter;
		private IConfigurationStore<System.Enum> _configurationStore;
		private IPortSender _portSender;
		private TcpServerListenerThread _tcpServerListenerThread;

		public TcpListenerRealTimeDataBusServer(IStarter starter, IPortSender portSender, IConfigurationStore<System.Enum> configurationStore) : base(portSender, configurationStore, starter)
		{
			_starter = starter;
			_configurationStore = configurationStore;
			_portSender = portSender;
		}

		public async Task DisplayFrameAsync(List<ScreenBuffer> screenBuffers)
		{
			await _tcpServerListenerThread.SendObject(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(screenBuffers)));
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		private void OnMessage(byte[] bytes)
		{
			int x = 4;
		}

		public Task PlaySound(byte[] data)
		{
			throw new NotImplementedException();
		}

		protected override bool StartConnection(int port)
		{
			_tcpServerListenerThread = new TcpServerListenerThread(OnMessage, port, 50);
			return true;
		}
	}
}
