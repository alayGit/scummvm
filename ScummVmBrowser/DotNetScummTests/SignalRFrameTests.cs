using CliScummEvents;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScummVMBrowser.Clients;
using ScummVMBrowser.StaticData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetScummTests
{
    //TODO: This class is making things directly insert unity
    [Ignore]
    [TestClass]
    public class SignalRFrameTests : FrameCaptureTests
    {
        HubConnection _connection;
        private int _noFrames;
        private string _expectedFrameName;
        private IHubProxy _proxy;


        public SignalRFrameTests() : base()
        { 
        }

        [TestInitialize]
        public void CreateClient()
        {
            ScummVMServerStarter starter = new ScummVMServerStarter();

            //int port = starter.StartScummVM("C:\\ScummVMNew\\ScummVMWeb\\GIT\\dists\\SignalRSelfHost\\bin\\Debug\\SignalRSelfHost.exe");
  
            _connection = new HubConnection($"http://localhost:{0}/");
            _proxy = _connection.CreateHubProxy("ScummHub");
            _proxy.On("NextFrame", (byte[] picBuff, int x, int y, int w, int h) => CaptureAndQuit(picBuff, x, y, w, h, _noFrames, _expectedFrameName));
            _connection.Start().Wait();
        }

        [TestMethod]
        public async Task CanStart()
        {
            _noFrames = 100;
            _expectedFrameName = "CanStart";
            await _proxy.Invoke("RunGame", "kq3", "");
            RunGame();
            await WaitForExpectedFrameAndQuit(_expectedFrameName, _noFrames, _proxy.Invoke("WaitForQuit"));
        }

        [TestMethod]
        public async Task CanSendEnter()
        {
            _expectedFrameName = "CanSendEnter";
            _noFrames = 150;
            await _proxy.Invoke("RunGame", "kq3", "");
            RunGame();
            await WaitForFrame(10);
            await _proxy.Invoke("EnqueueString", "\r");
            await WaitForExpectedFrameAndQuit(_expectedFrameName, _noFrames, _proxy.Invoke("WaitForQuit"));
        }

        [TestMethod]
        public async Task CanSendString()
        {
            _expectedFrameName = "CanSendString";
            _noFrames = 500;
            await _proxy.Invoke("RunGame", "kq3", "");
            RunGame();
            await WaitForFrame(10);
            await _proxy.Invoke("EnqueueString", "\r");
            await WaitAdditionalFrames(10);
            await _proxy.Invoke("EnqueueString", "AbcdEFg");
            await WaitAdditionalFrames(10);
            await _proxy.Invoke("EnqueueString", "\r");
            await WaitAdditionalFrames(10);
            await _proxy.Invoke("EnqueueString", "\r");
            await WaitForExpectedFrameAndQuit(_expectedFrameName, _noFrames, _proxy.Invoke("WaitForQuit"));
        }

        protected async override void Quit()
        {
            await _proxy.Invoke("Quit");
        }

        private void RunGame()
        {
           _proxy.Invoke("StartGame");
        }

        protected override async Task WaitForExpectedFrameAndQuit(string expectedFrameName, int noFrames, Task completeWhenQuit, int delay = 20000, int quitDelay = 20000)
        {
            await base.WaitForExpectedFrameAndQuit(expectedFrameName, noFrames, completeWhenQuit, delay, quitDelay);
            _connection.Stop();
        }
    }
}