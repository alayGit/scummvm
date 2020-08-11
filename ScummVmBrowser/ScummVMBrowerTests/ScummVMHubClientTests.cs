using ConfigStore;
using ManagedCommon.Base;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScummVMBrowerTests;
using ScummVMBrowser.Clients;
using SignalRHostWithUnity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace ScummVMBrowserTests
{
    [TestClass]
    public class ScummVMHubClientTests
    {
        UnityContainer _container;
        MockHubProxyConnection _connection;

        [TestInitialize]
        public void init()
        {
            _container = new UnityContainer();
            _container.RegisterType<IScummVMHubClient, ScummVMHubClient>();
            _container.RegisterType<IScummVMServerStarter, MockScummVMStarter>();
            _container.RegisterType<IHubConnectionFactory, MockHubConnectionFactory>();
            _container.RegisterType<IHubProxyConnection, MockHubProxyConnection>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IHubProxy, MockHubProxy>();
            _container.RegisterType<IConfigurationStore<System.Enum>, ConfigStore.JsonConfigStore>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IRealTimeDataEndpointClient, MockRealTimeDataEndpoint>();

            _connection = (MockHubProxyConnection)_container.Resolve<IHubProxyConnection>();
        }


        [TestMethod]
        public async Task CanStart()
        {
            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                 await client.StartGame(new Dictionary<string, byte[]>(), AvailableGames.kq3);

                Assert.IsTrue(client.HasGameStarted);
            }
        }

        [TestMethod]
        public async Task CanQuit()
        {
            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                await client.StartGame(new Dictionary<string, byte[]>(), AvailableGames.kq3);
                await client.Quit();

                Assert.IsTrue(client.HasFrontEndQuit);
            }
        }

        [TestMethod]
        public async Task CanEnqueueStrings()
        {
            const string TestString1 = "Test";
            const string TestString2 = "Test2";

            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                await client.StartGame(new Dictionary<string, byte[]>(), AvailableGames.kq3);
                await client.EnqueueString(TestString1);
                await client.EnqueueString(TestString2);

                List<string> enqueuedStrings = ((MockHubProxy)_connection.HubProxy).EnqueuedStrings;

                Assert.IsTrue(client.HasGameStarted);
                Assert.AreEqual(enqueuedStrings[0], TestString1);
                Assert.AreEqual(enqueuedStrings[1], TestString2);
                Assert.AreEqual(enqueuedStrings.Count, 2);
            }
        }

        [TestMethod]
        public async Task CanEnqueueControlKeys()
        {
            const ControlKeys TestControlKeys1 = ControlKeys.ArrowDown;
            const ControlKeys TestControlKeys2 = ControlKeys.ArrowLeft;

            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                await client.StartGame(new Dictionary<string, byte[]>(), AvailableGames.kq3);
                await client.EnqueueControlKey(TestControlKeys1);
                await client.EnqueueControlKey(TestControlKeys2);

                List<ControlKeys> enqueuedControlKeys = ((MockHubProxy)_connection.HubProxy).EnqueuedControlKeys;

                Assert.IsTrue(client.HasGameStarted);
                Assert.AreEqual(enqueuedControlKeys[0], TestControlKeys1);
                Assert.AreEqual(enqueuedControlKeys[1], TestControlKeys2);
                Assert.AreEqual(enqueuedControlKeys.Count, 2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CannotStartWithNoConnection()
        {
            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                 await client.StartGame(new Dictionary<string, byte[]>(), AvailableGames.kq3);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CannotQuitWhileNotStarted()
        {
            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                await client.Quit();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CannotEnqueueWhenNotStarted()
        {
            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                await client.EnqueueString("blah");
            }
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CannotStartTwice()
        {
            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                 await client.StartGame(new Dictionary<string, byte[]>(), AvailableGames.kq3);
                 await client.StartGame(new Dictionary<string, byte[]>(), AvailableGames.kq3);
            }
        }

        [TestMethod]
        public async Task QuitsOnBeforeDispose()
        {
            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                await client.StartGame(new Dictionary<string, byte[]>(), AvailableGames.kq3);
                await client.BeforeDispose();

                Assert.IsTrue(client.HasFrontEndQuit);
            }
        }

        [TestMethod]
        public async Task DisposesConnectionOnDispose()
        {
            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                 await client.StartGame(new Dictionary<string, byte[]>(), AvailableGames.kq3);
            }
            Assert.IsTrue(_connection.IsDisposed);
        }

        [Ignore]
        [TestMethod]
        public async Task CallsBackWhenFrameIsUpdated()
        {
            bool isCalled = false;
            byte[] expectedPicBuff = new byte[] { 12, 3, 13 };
            int expectedX = 5;
            int expectedY = 10;
            int expectedW = 6;
            int expectedH = 12;

            using (IScummVMHubClient client = _container.Resolve<IScummVMHubClient>())
            {
                client.SetNextFrameFunctionPointer((List<ScreenBuffer> screenBuffers) =>
                {
                    Assert.IsTrue(expectedPicBuff.SequenceEqual(screenBuffers[0].Buffer));
                    Assert.AreEqual(expectedX, screenBuffers[0].X);
                    Assert.AreEqual(expectedY, screenBuffers[0].Y);
                    Assert.AreEqual(expectedW, screenBuffers[0].W);
                    Assert.AreEqual(expectedH, screenBuffers[0].H);

                    isCalled = true;

                    return Task.CompletedTask;
                });
                 await client.StartGame(new Dictionary<string, byte[]>(), AvailableGames.kq3);

                Subscription subscription = ((MockHubProxy)_connection.HubProxy).Subscription;
                
                MethodInfo dynMethod = subscription.GetType().GetMethod("OnReceived",
                BindingFlags.NonPublic | BindingFlags.Instance);

                JArray jArray = new JArray();
                
                JObject obj = JObject.Parse("{X:5, Y:10, W:6, H:12, Buffer: [12, 3, 13]}");
                jArray.Add(obj);

                

            dynMethod.Invoke(subscription, new object[] { jArray });

                Assert.IsTrue(isCalled);
            }

        }

        //internal static void Raise<TEventArgs>(this object source, string eventName, TEventArgs eventArgs) where TEventArgs : EventArgs
        //{
        //    var eventDelegate = (MulticastDelegate)source.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(source);
        //    if (eventDelegate != null)
        //    {
        //        foreach (var handler in eventDelegate.GetInvocationList())
        //        {
        //            handler.Method.Invoke(handler.Target, new object[] { source, eventArgs });
        //        }
        //    }
        //}  new ScreenBuffer() { Buffer = expectedPicBuff, X = expectedX, Y = expectedY, W = expectedW, H = expectedH }

    }
}
