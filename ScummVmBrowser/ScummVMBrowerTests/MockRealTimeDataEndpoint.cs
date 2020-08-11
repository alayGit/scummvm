using ManagedCommon.Base;
using ManagedCommon.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowerTests
{
    class MockRealTimeDataEndpoint : IRealTimeDataEndpointClient
    {
        public void Dispose()
        {
        }

        public async Task Init(string id)
        {
            await Task.CompletedTask;
        }

        public void OnAudioReceived(PlayAudioAsync playAudio, int instanceId)
        {
        }

        public void OnFrameReceived(CopyRectToScreenAsync copyRectToScreen, int instanceId)
        {
        }

        public async Task StartConnectionAsync()
        {
            await Task.CompletedTask;
        }
    }
}
