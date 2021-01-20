using ManagedCommon.Base;
using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Settings;
using ManagedCommon.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	public class TcpListenerRealTimeDataEndpointServer : IRealTimeDataEndpointServer
	{
		private IStarter _starter;
		private IConfigurationStore<System.Enum> _configurationStore;
		private TcpListenerThread _tcpClientListenerThread;

		private EnqueueControlKey _enqueueControlKey;
		private EnqueueMouseClick _enqueueMouseClick;
		private EnqueueMouseMove _enqueueMouseMove;
		private EnqueueString _enqueueString;
		private ScheduleRedrawWholeScreen _scheduleRedrawWholeScreen;
		private StartSound _startSound;
		private StopSound _stopSound;
		private IPortSender _portSender; 

		public TcpListenerRealTimeDataEndpointServer(IStarter starter, IConfigurationStore<System.Enum> configurationStore, IPortSender portSender)
		{
			_starter = starter;
			_configurationStore = configurationStore;
			_portSender = portSender;
		}


		public void Dispose()
		{
		}

		public Task Init(string realTimePortGetterId)
		{
			int port = 0;

			bool result = _starter.StartConnection(
			   p =>
			   {
				   port = p;
				   _tcpClientListenerThread = Singleton.GetTcpListener(port, IPAddress.Parse(_configurationStore.GetValue(TcpListenerSettings.ServerIp)), ListenerTypeEnum.Server);
				   _tcpClientListenerThread.RunTcpListenerTask(OnMessage, _configurationStore.GetValue<int>(TcpListenerSettings.ServerSleepTime));

				   Console.WriteLine($"Tcp Server Started On Port {p}.");

				   return true;
			   }
			 );

			if (!result)
			{
				throw new Exception($"Failed to start signalR webServer on port {port}.");
			}

			_portSender.Init(() => port, realTimePortGetterId);

			return Task.CompletedTask;
		}

		public void OnEnqueueControlKey(EnqueueControlKey enqueueControlKey)
		{
			_enqueueControlKey = enqueueControlKey;
		}

		public void OnEnqueueMouseClick(EnqueueMouseClick enqueueMouseClick)
		{
			_enqueueMouseClick = enqueueMouseClick;
		}

		public void OnEnqueueMouseMove(EnqueueMouseMove enqueueMouseMove)
		{
			_enqueueMouseMove = enqueueMouseMove;
		}

		public void OnEnqueueString(EnqueueString enqueueString)
		{
			_enqueueString = enqueueString;
		}

		public void OnScheduleRedrawWholeScreen(ScheduleRedrawWholeScreen scheduleRedrawWholeScreen)
		{
			_scheduleRedrawWholeScreen = scheduleRedrawWholeScreen;
		}

		public void OnStartSound(StartSound startSound)
		{
			_startSound = startSound;
		}

		public void OnStopSound(StopSound stopSound)
		{
			_stopSound = stopSound;
		}


		public async Task OnMessage(byte[] value)
		{
			MessageTypeEnum messageType;
			string message = Helpers.GetMessage(value, out messageType);

			switch (messageType)
			{
				case MessageTypeEnum.EnqueueControlKey:
					_enqueueControlKey?.Invoke((ControlKeys)int.Parse(message));
					break;

				case MessageTypeEnum.EnqueueMouseClick:
					_enqueueMouseClick?.Invoke((MouseClick)int.Parse(message));
					break;

				case MessageTypeEnum.EnqueueMouseMove:
					int[] mouseCoords = JsonConvert.DeserializeObject<int[]>(message);

					if (mouseCoords.Length != 2)
					{
						throw new Exception($"Fail mouse coords with {mouseCoords.Length} elements instead of 2");
					}

					_enqueueMouseMove?.Invoke(mouseCoords[0], mouseCoords[1]);
					break;

				case MessageTypeEnum.EnqueueString:
					_enqueueString(message);
					break;

				case MessageTypeEnum.ScheduleRedrawWholeScreen:
					_scheduleRedrawWholeScreen();
					break;

				case MessageTypeEnum.StartSound:
					_startSound();
					break;

				case MessageTypeEnum.StopSound:
					_stopSound();
					break;

				default:
					throw new Exception("Fail: Unknown Message MessageTypeEnum");
			}
			await Task.CompletedTask;
		}
	}
}
