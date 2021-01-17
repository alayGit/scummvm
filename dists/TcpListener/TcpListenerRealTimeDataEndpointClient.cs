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
		private PlayAudioAsync _playAudio;

		public TcpListenerRealTimeDataEndpointClient(IStarter starter, IConfigurationStore<System.Enum> configurationStore)
		{
			_starter = starter;
			_configurationStore = configurationStore;
		}


		public void Dispose()
		{
		}

		public async Task Init(string port)
		{
			_tcpClientListenerThread = Singleton.GetTcpListener(int.Parse(port), ListenerTypeEnum.Client);
			_tcpClientListenerThread.RunTcpListenerTask(OnMessage, 3);

			await Task.CompletedTask;
		}

		public void OnAudioReceived(PlayAudioAsync playAudio, int instanceId)
		{
			_playAudio = playAudio;
		}

		public void OnFrameReceived(CopyRectToScreenAsync copyRectToScreen, int instanceId)
		{
			_copyRectToScreen = copyRectToScreen;
		}

		public async Task OnMessage(byte[] value)
		{
			MessageTypeEnum messageType;
			string message = Helpers.GetMessage(value, out messageType);
			

			switch (messageType)
			{
				case MessageTypeEnum.OnFrameReceived:
					if(_copyRectToScreen != null)
					{
						await _copyRectToScreen(JsonConvert.DeserializeObject<List<ScreenBuffer>>(message));
					}
					break;
				case MessageTypeEnum.OnPlaySound:
					if(_playAudio != null)
					{
						await _playAudio(Convert.FromBase64String(message));
					}
					break;
				default:
					throw new Exception("Fail: Unknown Message MessageTypeEnum");
			}

			
		}

		public Task StartConnectionAsync()
		{
			throw new NotImplementedException();
		}
	}
}
