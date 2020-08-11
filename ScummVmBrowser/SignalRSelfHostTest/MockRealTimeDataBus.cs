using ManagedCommon.Enums.Actions;
using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMocks;

namespace SignalRSelfHostTest
{
    public class MockRealTimeDataBus : IRealTimeDataBusServer
    {
        public MockNextFrame NextFrame { get; set; }

        public Task DisplayFrameAsync(List<ScreenBuffer> screenBuffers)
        {
            NextFrame.NextFrame(screenBuffers);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
 
        }

        public Task EnqueueControlKeyAsync(ManagedCommon.Enums.ControlKeys toSend)
        {
            throw new NotImplementedException();
        }

        public Task EnqueueMouseClickAsync(MouseClick mouseClick)
        {
            throw new NotImplementedException();
        }

        public Task EnqueueMouseMoveAsync(int x, int y)
        {
            throw new NotImplementedException();
        }

        public Task EnqueueStringAsync(string toSend)
        {
            throw new NotImplementedException();
        }

        public async Task Init(string id)
        {
            await Task.CompletedTask;
        }

        public Task PlaySound(byte[] data)
        {
            return Task.CompletedTask;
        }

        public Task StartSoundAsync()
        {
            throw new NotImplementedException();
        }

        public Task StopSoundAsync()
        {
            throw new NotImplementedException();
        }
    }
}