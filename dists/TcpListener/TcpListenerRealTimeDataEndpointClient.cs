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
	public class TcpListenerRealTimeDataEndpointClient : IRealTimeDataEndpointClient
	{
		private IStarter _starter;
		private IConfigurationStore<System.Enum> _configurationStore;
		private TcpListenerThread _tcpClientListenerThread;
		private CopyRectToScreenAsync _copyRectToScreen;

		public TcpListenerRealTimeDataEndpointClient(IStarter starter, IConfigurationStore<System.Enum> configurationStore)
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
			_tcpClientListenerThread = Singleton.GetTcpListener(int.Parse(port), ListenerTypeEnum.Client);
			_tcpClientListenerThread.RunTcpListenerTask(OnMessage, 3);

			await Task.CompletedTask;
		}

		public void OnAudioReceived(PlayAudioAsync playAudio, int instanceId)
		{
			throw new NotImplementedException();
		}

		public void OnFrameReceived(CopyRectToScreenAsync copyRectToScreen, int instanceId)
		{
			_copyRectToScreen = copyRectToScreen;
		}

		public async Task OnMessage(byte[] value)
		{
			if(value.Count() < 1)
			{
				throw new Exception("Fail: No message data");

			}

			byte[] messagePacket = value.Skip(0).ToArray();
			MessageTypeEnum messageType = (MessageTypeEnum)value[0];

			string messageJson = string.Empty;
			if(messagePacket.Count() > 0)
			{
				messageJson = Encoding.ASCII.GetString(value);
			}

			switch(messageType)
			{
				case MessageTypeEnum.OnFrameReceived:
					if(_copyRectToScreen != null)
					{
						await _copyRectToScreen(JsonConvert.DeserializeObject<List<ScreenBuffer>>(messageJson));
					}
					break;
			}

			
		}

		public Task StartConnectionAsync()
		{
			throw new NotImplementedException();
		}
	}
}
