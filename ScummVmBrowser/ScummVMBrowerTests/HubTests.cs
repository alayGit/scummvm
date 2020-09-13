using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScummVMBrowser.Clients;
using ScummVMBrowser.Data;
using ScummVMBrowser.Models;
using ScummVMBrowser.Server;
using SignalRHostWithUnity;
using SignalRSelfHostTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace ScummVMBrowerTests
{
    [TestClass]
    public class HubTests
    {
        UnityContainer _container;
        HubServer _hubServer;
        MockScummVMHubClient _scummVMClient;
        MockClient _frontEndClient;

        [TestInitialize]
        public void init()
        {
            _container = new UnityContainer();
            _scummVMClient = new MockScummVMHubClient();
            _frontEndClient = new MockClient();

            _container.RegisterInstance<IScummVMHubClient>(_scummVMClient);
            _container.RegisterType<HubServer, HubServer>();
            _container.RegisterType<IGameClientStore<IGameInfo>, MockHubStore>();
            _container.RegisterType<IRequest, MockRequest>();
            _container.RegisterType<IHubConnectionFactory, MockHubConnectionFactory>();
            _container.RegisterType<IHubProxyConnection, MockHubProxyConnection>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IHubProxy, MockHubProxy>();
            _container.RegisterInstance<IHubCallerConnectionContext<dynamic>>(_frontEndClient);
            _container.RegisterType<IGameInfo, MockGameInfo>();

            _hubServer = _container.Resolve<HubServer>();
            _hubServer.Context = new HubCallerContext(_container.Resolve<IRequest>(), "12345");
            _hubServer.Clients = _container.Resolve<IHubCallerConnectionContext<dynamic>>();
        }

        [TestMethod]
        public async Task CanSendFramesBackToClient()
        {
            IEnumerable<ExpectedNextFrameResult> expectedNextFrameResults = new List<ExpectedNextFrameResult>()
            {
                new ExpectedNextFrameResult() {PicBuff = new byte[] {1,1,45,3,1}, H = 4, W = 3, X = 4, Y = 5},
                new ExpectedNextFrameResult() {PicBuff = new byte[] {2,6,6,7,9}, H = 74, W = 73, X = 74, Y = 75}
            };

            _frontEndClient.NextFrame = new MockNextFrame(expectedNextFrameResults);

            using (_hubServer)
            {
                await _hubServer.Init(Guid.NewGuid().ToString());

                Assert.IsTrue(_scummVMClient.ConnectionStarted);
                Assert.IsTrue(_scummVMClient.GameStarted);

                foreach (ExpectedNextFrameResult expectedNextFrameResult in expectedNextFrameResults)
                {
                    List<ScreenBuffer> screenBuffers = new List<ScreenBuffer>() { new ScreenBuffer() { Buffer = expectedNextFrameResult.PicBuff, H = expectedNextFrameResult.H, W = expectedNextFrameResult.W, X = expectedNextFrameResult.X, Y = expectedNextFrameResult.Y } };
                    _scummVMClient.ScreenDrawingCallback(screenBuffers);
                }
            }

            Assert.IsTrue(_frontEndClient.NextFrame.NextFrameCalledCorrectNumberOfTimes);
            Assert.IsTrue(_frontEndClient.NextFrame.EveryFrameMatched);
        }


        [TestMethod]
        public async Task CanEnqueueString()
        {
            const string TestString1 = "Fish";
            const string TestString2 = "N Chips";
       
            using (_hubServer)
            {
                await _hubServer.Init(Guid.NewGuid().ToString());
                await _hubServer.EnqueueString(TestString1);
                await _hubServer.EnqueueString(TestString2);
            }

            Assert.IsTrue(_scummVMClient.EnqueuedStrings.Contains(TestString1));
            Assert.IsTrue(_scummVMClient.EnqueuedStrings.Contains(TestString2));
            Assert.AreEqual(2, _scummVMClient.EnqueuedStrings.Count);
        }

        [TestMethod]
        public async Task CanEnqueueControlKey()
        {
            const  ControlKeys TestCtrlKey1 = ControlKeys.ArrowDown;
            const ControlKeys TestCtrlKey2 = ControlKeys.ArrowLeft;

            using (_hubServer)
            {
                await _hubServer.Init(Guid.NewGuid().ToString());
                await _hubServer.EnqueueControlKey(TestCtrlKey1);
                await _hubServer.EnqueueControlKey(TestCtrlKey2);
            }

            Assert.IsTrue(_scummVMClient.EnqueuedControlKeys.Contains(TestCtrlKey1));
            Assert.IsTrue(_scummVMClient.EnqueuedControlKeys.Contains(TestCtrlKey2));
            Assert.AreEqual(2, _scummVMClient.EnqueuedControlKeys.Count);
        }

        [TestMethod]
        public async Task CanQuitWhileGameIsRunning()
        {
            using (_hubServer)
            {
                await _hubServer.Init(Guid.NewGuid().ToString());
                await _hubServer.Quit();
            }

            Assert.IsTrue(_scummVMClient.HasFrontEndQuit);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CannotQuitTwice()
        {
            using (_hubServer)
            {
                await _hubServer.Init(Guid.NewGuid().ToString());
                await _hubServer.Quit();
                await _hubServer.Quit();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CannotQuitWhileNotRunning()
        {
            using (_hubServer)
            {
                await _hubServer.Quit();
            }
        }
    }
}
