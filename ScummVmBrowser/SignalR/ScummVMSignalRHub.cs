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

        public async Task PlaySound(byte[] data)
        {
            await Clients.Client(CurrentConnectionId).PlayAudio(data);
        }

        public void EnqueueControlKey(ControlKeys controlKey)
        {
            _realTimeEndPointCallbackRepo.EnqueueControlKey(controlKey);
        }

        public void EnqueueMouseClick(MouseClick mouseClick)
        {
            _realTimeEndPointCallbackRepo.EnqueueMouseClick(mouseClick);
        }

        public void EnqueueMouseMove(int x, int y)
        {
            _realTimeEndPointCallbackRepo.EnqueueMouseMove(x, y);
        }

        public void EnqueueString(string stringToEnqueue)
        {
            _realTimeEndPointCallbackRepo.EnqueueString(stringToEnqueue);
        }

        public List<ScreenBuffer> GetWholeScreenBuffer()
        {
            return _realTimeEndPointCallbackRepo.GetRedrawWholeScreenBuffersCompressed();
        }

        public void StartSound()
        {
            _realTimeEndPointCallbackRepo.StartSound();
        }

        public void StopSound()
        {
            _realTimeEndPointCallbackRepo.StopSound();
        }

        public async Task DisplayFrameAsync(List<ScreenBuffer> screenBuffers)
        {
            await Clients.Client(CurrentConnectionId).NextFrame(screenBuffers);
        }
    }
}
