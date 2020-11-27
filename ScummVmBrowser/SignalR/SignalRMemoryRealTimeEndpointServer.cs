using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
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
        public EnqueueString EnqueueStringCallback { get; private set; }

        public EnqueueControlKey EnqueueControlKey { get; private set; }

        public EnqueueMouseMove EnqueueMouseMove { get; private set; }

        public EnqueueString EnqueueString { get; private set; }

        public StartSound StartSound { get; private set; }

        public StopSound StopSound { get; private set; }

        public GetRedrawWholeScreenBuffersCompressed GetRedrawWholeScreenBuffersCompressed { get; private set; }

        public EnqueueMouseClick EnqueueMouseClick { get; private set; }

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

        public void OnEnqueueControlKey(EnqueueControlKey enqueueControlKey)
        {
            EnqueueControlKey = enqueueControlKey;
        }

        public void OnEnqueueMouseClick(EnqueueMouseClick enqueueMouseClick)
        {
            EnqueueMouseClick = enqueueMouseClick;
        }

        public void OnEnqueueMouseMove(EnqueueMouseMove enqueueMouseMove)
        {
            EnqueueMouseMove = enqueueMouseMove;
        }

        public void OnEnqueueString(EnqueueString enqueueString)
        {
            EnqueueString = enqueueString;
        }

        public void OnGetRedrawWholeScreenBuffersCompressed(GetRedrawWholeScreenBuffersCompressed getWholeScreenBuffer)
        {
			GetRedrawWholeScreenBuffersCompressed = getWholeScreenBuffer;
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
