using ManagedCommon.Base;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Settings;
using ManagedCommon.Interfaces;
using Newtonsoft.Json;
using PortSharer;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	public class TcpListenerRealTimeDataBusClient : IRealTimeDataBusClient
	{

		private IStarter _starter;
		private IConfigurationStore<System.Enum> _configurationStore;
		private IPortSender _portSender;
		private TcpListenerThread _tcpClientListenerThread;

		public TcpListenerRealTimeDataBusClient(IStarter starter, IPortSender portSender, IConfigurationStore<System.Enum> configurationStore)
		{
			_starter = starter;
			_configurationStore = configurationStore;
			_portSender = portSender;
		}

		public void Dispose()
		{
		}

		public async Task EnqueueControlKeyAsync(ControlKeys toSend)
		{
			await _tcpClientListenerThread.SendAsciiBytes(Helpers.ConvertObjectToMessage(toSend, MessageTypeEnum.EnqueueControlKey));
		}

		public async Task EnqueueMouseClickAsync(MouseClick mouseClick)
		{
			await _tcpClientListenerThread.SendAsciiBytes(Helpers.ConvertObjectToMessage(mouseClick, MessageTypeEnum.EnqueueMouseClick));
		}

		public async Task EnqueueMouseMoveAsync(int x, int y)
		{
			await _tcpClientListenerThread.SendAsciiBytes(Helpers.ConvertObjectToMessage($"[{x},{y}]", MessageTypeEnum.EnqueueMouseMove));
		}

		public async Task EnqueueStringAsync(string toSend)
		{
			await _tcpClientListenerThread.SendAsciiBytes(Helpers.ConvertObjectToMessage(toSend, MessageTypeEnum.EnqueueString));
		}

		public Task Init(string port)
		{
			_tcpClientListenerThread = Singleton.GetTcpListener(int.Parse(port), IPAddress.Parse(_configurationStore.GetValue(TcpListenerSettings.ServerIp)), ListenerTypeEnum.Client);
			return Task.CompletedTask;
		}

		public async Task<List<ScreenBuffer>> ScheduleRedrawWholeScreen()
		{
			return await Task.FromResult(new List<ScreenBuffer>());
			//await _tcpServerListenerThread.SendAsciiBytes(Helpers.ConvertObjectToMessage($"[]", MessageTypeEnum.ScheduleRedrawWholeScreen));
		}

		public Task StartConnectionAsync()
		{
			return Task.CompletedTask;
		}

		public async Task StartSoundAsync()
		{
			await _tcpClientListenerThread.SendAsciiBytes(Helpers.CreateMessageWithNoContents(MessageTypeEnum.StartSound));
		}

		public async Task StopSoundAsync()
		{
			await _tcpClientListenerThread.SendAsciiBytes(Helpers.CreateMessageWithNoContents(MessageTypeEnum.StopSound));
		}
	}
}
