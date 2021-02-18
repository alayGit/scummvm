using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ManagedCommon.Models;
using Microsoft.Owin.Hosting;
using PortSharer;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR
{
    public class SignalRMemoryRealTimeEndpointServer : IRealTimeDataEndpointServer, IRealTimeEndPointCallbackRepo
    {
		public EnqueueInputMessages EnqueueInputMessages { get; private set; }

        public StartSound StartSound { get; private set; }

        public StopSound StopSound { get; private set; }

        public ScheduleRedrawWholeScreen ScheduleRedrawWholeScreen { get; private set; }

        private IStarter _starter;
        private IDisposable _webApp; //TODO: Dispose
        private IConfigurationStore<System.Enum> _configurationStore;
        private IPortSender _portSender;

        public SignalRMemoryRealTimeEndpointServer(IStarter starter, IPortSender portSender, IConfigurationStore<System.Enum> configurationStore)
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
            string hostName = _configurationStore.GetValue(ScummConnectionSettings.CliScummHubHostName);

            bool result = _starter.StartConnection(
               p =>
               {
                   port = p;

                   _webApp = WebApp.Start($"http://{hostName}:{port}");
                   return true;
               }
             );

            if (!result)
            {
                throw new Exception($"Failed to start signalR webServer on port {port}.");
            }

            _portSender.Init(() => port, realTimePortGetterId);

            string url = $"http://{hostName}:{port}";

            Console.WriteLine($"Server running on {port}", url);

            return Task.CompletedTask;
        }

		public void OnEnqueueInputMessages(EnqueueInputMessages enqueueInputMessages)
		{
			EnqueueInputMessages = enqueueInputMessages;
		}

		public void OnScheduleRedrawWholeScreen(ScheduleRedrawWholeScreen getWholeScreenBuffer)
        {
			ScheduleRedrawWholeScreen = getWholeScreenBuffer;
        }

        public void OnStartSound(StartSound startSound)
        {
            StartSound = startSound;
        }

        public void OnStopSound(StopSound stopSound)
        {
            StopSound = stopSound;
        }
	}
}
