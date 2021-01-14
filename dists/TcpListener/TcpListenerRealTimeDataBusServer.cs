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
		private TcpListenerThread _tcpServerListenerThread;

		public TcpListenerRealTimeDataBusServer(IStarter starter, IPortSender portSender, IConfigurationStore<System.Enum> configurationStore) : base(portSender, configurationStore, starter)
		{
			_starter = starter;
			_configurationStore = configurationStore;
			_portSender = portSender;
		}

		public async Task DisplayFrameAsync(List<ScreenBuffer> screenBuffers)
		{
			await _tcpServerListenerThread.SendTerminatedAsciiBytes(ConvertObjectToMessage(screenBuffers, MessageTypeEnum.OnFrameReceived));
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		private Task OnMessage(byte[] bytes)
		{
			return Task.CompletedTask;
		}

		public async Task PlaySound(byte[] data)
		{
			await _tcpServerListenerThread.SendTerminatedAsciiBytes(ConvertObjectToMessage(data, MessageTypeEnum.OnPlaySound));
		}

		protected override bool StartConnection(int port)
		{
			_tcpServerListenerThread = Singleton.GetTcpListener(port, ListenerTypeEnum.Server);
			return true;
		}

		private byte[] ConvertObjectToMessage(object toSerialize, MessageTypeEnum messageTypeEnum)
		{
			string serialized = JsonConvert.SerializeObject(toSerialize) + (char)0;

			byte[] asciiSerializedBytes = Encoding.ASCII.GetBytes(serialized);
			byte[] asciiSerializedBytesWithMessage = new byte[asciiSerializedBytes.Length + 1];
			asciiSerializedBytesWithMessage[0] = (byte)MessageTypeEnum.OnFrameReceived;
			Array.Copy(asciiSerializedBytes, 0, asciiSerializedBytesWithMessage, 1, asciiSerializedBytes.Length);

			return asciiSerializedBytes;
		}
	}
}
