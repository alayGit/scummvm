using System;
using ManagedCommon.Delegates;
using ManagedCommon.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PortSharer;
using StartInstance;
using TcpRealTimeData;
using System.Collections.Generic;
using ManagedZLibCompression;
using System.Threading.Tasks;

namespace TcpListenerRealTimeTest
{
    [TestClass]
    public class TcpListenerRealTimeDataBusServerTests
    {
		TcpListenerRealTimeDataBusServer _tcpListenerRealTimeDataBusServer;
		TcpListenerRealTimeDataEndpointServer _tcpListenerRealTimeDataEndpointServer;
		static ManagedZLibCompression.ManagedZLibCompression compressor = new ManagedZLibCompression.ManagedZLibCompression();

		const int Port = 8256;

		[TestInitialize]
		public void Init()
		{
			Mock<IPortSender> mockPortSender = new Mock<IPortSender>();
			mockPortSender.Setup(m => m.Init(It.IsAny<GetPort>(), It.IsAny<string>()));

			Mock<IStarter> mockStarter = new Mock<IStarter>();

			mockStarter.Setup(s => s.StartConnection(It.IsAny<StartConnection>())).Callback((StartConnection sc) => sc(Port)).Returns(true);

			Mock<IConfigurationStore<Enum>> mockConfigurationStore = new Mock<IConfigurationStore<Enum>>();

			_tcpListenerRealTimeDataBusServer = new TcpListenerRealTimeDataBusServer(mockStarter.Object, mockPortSender.Object, mockConfigurationStore.Object);
			_tcpListenerRealTimeDataEndpointServer = new TcpListenerRealTimeDataEndpointServer(mockStarter.Object, mockConfigurationStore.Object);
			_tcpListenerRealTimeDataBusServer.Init("Blah").Wait();
			_tcpListenerRealTimeDataEndpointServer.Init(Port.ToString()).Wait();
		}

        [TestMethod]
        public async Task CanSendScreenBuffers()
        {
			List<ScreenBuffer> screenBuffers = new List<ScreenBuffer>()
			{
				new ScreenBuffer() { X = 12, Y = 17, CompressedBuffer = compressor.Compress(new byte[] { 12,34,5,55 }) }
			};

			List<ScreenBuffer> screenBuffers2 = new List<ScreenBuffer>()
			{
				new ScreenBuffer() { X = 16, Y = 16, CompressedBuffer = compressor.Compress(new byte[] { 11,36,0,15 }) }
			};
			await _tcpListenerRealTimeDataBusServer.DisplayFrameAsync(screenBuffers);
			await _tcpListenerRealTimeDataBusServer.DisplayFrameAsync(screenBuffers2);

			await Task.Delay(500000);
        }
    }
}
