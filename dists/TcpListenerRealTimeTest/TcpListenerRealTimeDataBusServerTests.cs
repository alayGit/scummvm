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
using System.Linq;

namespace TcpListenerRealTimeTest
{
    [TestClass]
    public class TcpListenerRealTimeDataBusServerTests
    {
		TcpListenerRealTimeDataBusServer _tcpListenerRealTimeDataBusServer;
		TcpListenerRealTimeDataEndpointClient _tcpListenerRealTimeDataEndpointClient;
		TcpListenerRealTimeDataEndpointServer _tcpListenerRealTimeDataEndpointServer;
		static ManagedZLibCompression.ManagedZLibCompression compressor = new ManagedZLibCompression.ManagedZLibCompression();

		const int Port = 8256;

		List<ScreenBuffer> ScreenBuffers = new List<ScreenBuffer>()
			{
				new ScreenBuffer() { X = 12, Y = 17, H = 12, W = 23, IgnoreColour = 12, PaletteHash = 123, CompressedPaletteBuffer =  compressor.Compress(new byte[] { 1,24,65,65 }), CompressedBuffer = compressor.Compress(new byte[] { 12,34,5,55 }) }
			};

		List<ScreenBuffer> ScreenBuffers2 = new List<ScreenBuffer>()
			{
				new ScreenBuffer() { X = 16, Y = 16, CompressedBuffer = compressor.Compress(new byte[] { 11,36,0,15 }) }
			};

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
			_tcpListenerRealTimeDataEndpointClient = new TcpListenerRealTimeDataEndpointClient(mockStarter.Object, mockConfigurationStore.Object);
			_tcpListenerRealTimeDataBusServer.Init("Blah").Wait();
			_tcpListenerRealTimeDataEndpointServer.Init(Port.ToString()).Wait();
		}

        [TestMethod]
        public async Task CanSendScreenBuffers()
        {
			await _tcpListenerRealTimeDataBusServer.DisplayFrameAsync(ScreenBuffers);
			await _tcpListenerRealTimeDataBusServer.DisplayFrameAsync(ScreenBuffers2);

			int  noTimesCalled = 0;

			_tcpListenerRealTimeDataEndpointClient.OnFrameReceived(b =>
			{
				List<ScreenBuffer> expectedScreenBuffers = null;

				Assert.IsTrue(noTimesCalled < 2);

				if (noTimesCalled == 0)
				{
					expectedScreenBuffers = ScreenBuffers;
				}
				else
				{
					expectedScreenBuffers = ScreenBuffers2;
				}

				AssertScreenBuffersMatch(expectedScreenBuffers, b);

				noTimesCalled++;

				return Task.CompletedTask;
			}, 0);

			await Task.Delay(2000);

			Assert.IsTrue(noTimesCalled == 2);
        }

		private void AssertScreenBuffersMatch(List<ScreenBuffer> expectedList, List<ScreenBuffer> actualList)
		{
			Assert.IsTrue(
				expectedList.All(e => actualList.Any(a => a.X == e.X && a.Y == e.Y
				&& a.H == e.H
				&& a.W == e.W
				&& a.IgnoreColour == e.IgnoreColour
				&& a.PaletteHash == e.PaletteHash
				&& a.CompressedPaletteBuffer.SequenceEqual(e.CompressedPaletteBuffer)
				&& a.CompressedBuffer.SequenceEqual(e.CompressedBuffer)
				))
			);
			Assert.AreEqual(expectedList.Count, actualList.Count);
		}

    }
}
