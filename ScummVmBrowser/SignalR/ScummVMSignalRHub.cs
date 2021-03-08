using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ManagedCommon.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ManagedCommon.Enums.Actions;
using System.Drawing;
using ManagedCommon.Base;
using System.IO;
using ManagedCommon.Delegates;
using SignalR;
using ManagedCommon.Models;
using ManagedCommon.Enums.Other;

namespace SignalRSelfHost
{
    public class ScummVMSignalRHub : HubBase, IRealTimeDataBusServer
    {
        private IRealTimeEndPointCallbackRepo _realTimeEndPointCallbackRepo;
        public IWrapper Wrapper { get; private set; }
        
        public ScummVMSignalRHub(IConfigurationStore<System.Enum> configurationStore, IRealTimeEndPointCallbackRepo realTimeEndPointCallbackRepo) : base(configurationStore)
        {
            _realTimeEndPointCallbackRepo = realTimeEndPointCallbackRepo;
        }

		public void EnqueueInputMessages(KeyValuePair<string, string>[] inputMessages)
		{
			_realTimeEndPointCallbackRepo.EnqueueInputMessages(inputMessages.Select(kvp => new InputMessage() { InputType = (InputType)Enum.Parse(typeof(InputType), kvp.Key), Input = kvp.Value }).ToArray());
		}

        public void ScheduleRedrawWholeScreen()
        {
            _realTimeEndPointCallbackRepo.ScheduleRedrawWholeScreen();
        }

        public void StartSound()
        {
            _realTimeEndPointCallbackRepo.StartSound();
        }

        public void StopSound()
        {
            _realTimeEndPointCallbackRepo.StopSound();
        }

        public async Task SendGameMessagesAsync(string gameMessages)
        {
            await Clients.Client(CurrentConnectionId).SendGameMessages(gameMessages);
        }
    }
}
