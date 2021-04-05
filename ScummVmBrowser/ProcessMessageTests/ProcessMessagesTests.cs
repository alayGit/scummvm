using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Logging;
using ManagedCommon.Interfaces;
using MessageBuffering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SevenZCompression;

namespace ProcessMessageTests
{
    [TestClass]
    public class ProcessMessagesTests
    {
		ProcessMessages _processMessages;
		Mock<IConfigurationStore<System.Enum>> _configStore;
		Mock<ILogger> _logger;
		ManagedYEncoder.ManagedYEncoder _managedYEncoder;
		SevenZCompressor _compressor;

		[TestInitialize]
		public void ProcessMessages()
		{
			_logger = new Mock<ILogger>();
			_managedYEncoder = new ManagedYEncoder.ManagedYEncoder(_logger.Object, LoggingCategory.CliScummSelfHost);
			_compressor = new SevenZCompressor();

			_configStore = new Mock<IConfigurationStore<Enum>>();
			_configStore.Setup(c => c.GetValue<int>(It.Is<ScummHubSettings>(e => e == ScummHubSettings.BufferAndProcessSleepTime))).Returns(5);

			_processMessages = new ProcessMessages(_configStore.Object, _managedYEncoder, _logger.Object, _compressor);
		}

        [TestMethod]
        public async Task ProcessMessagesSendsMessages()
        {
			byte[] TestValue1 = new byte[] { 49, 57, 55, 50, 53, 51 };
			byte[] TestValue2 = new byte[] { 50, 50, 54, 49 };
			const string TestValue3 = "Test3";
			const string TestValue4 = "Test";

			int timesSeen1 = 0, timesSeen2 = 0, timesSeen3 = 0, timesSeen4 = 0;

			_processMessages.Enqueue(TestValue1, MessageType.Frames);
			_processMessages.Enqueue(TestValue2, MessageType.Frames);
			_processMessages.Enqueue(TestValue3, MessageType.Sound);
			_processMessages.Enqueue(TestValue4, MessageType.Sound);

			_processMessages.MessagesProcessed = async m =>
			{
				byte[] decodedMessages = _managedYEncoder.ByteDecode(m);
				byte[] decompressed = _compressor.Decompress(decodedMessages);
				KeyValuePair<MessageType, string[]>[] messages = JsonConvert.DeserializeObject<KeyValuePair<MessageType, string[]>[]>(_managedYEncoder.TextEncoding.GetString(decompressed));

				Assert.AreEqual(2, messages.Length);
			  	foreach(KeyValuePair<MessageType, string[]> kvp in messages)
				{
					if (kvp.Key == MessageType.Frames)
					{
					  List<byte[]> bTestDatas = kvp.Value.Select(y => _managedYEncoder.ByteDecode(y)).ToList();

					   foreach(byte[] testData in bTestDatas)
						{
							if(testData.SequenceEqual(TestValue1))
							{
								timesSeen1++;
							}
							else if (testData.SequenceEqual(TestValue2))
							{
								timesSeen2++;
							}
							else
							{
								throw new Exception("Failed unknown data");
							}

						}
					}
					else if(kvp.Key == MessageType.Sound)
					{
						foreach (string testData in kvp.Value)
						{
							if (testData == TestValue3)
							{
								timesSeen3++;
							}
							else if(testData == TestValue4)
							{ 
								timesSeen4++;
							}
							else
							{
								throw new Exception("Failed unknown data");
							}

						}
					}
				}
				await Task.CompletedTask;
			};
			await _processMessages.Stop();

			Assert.AreEqual(1, timesSeen1);
			Assert.AreEqual(1, timesSeen2);
			Assert.AreEqual(1, timesSeen3);
			Assert.AreEqual(1, timesSeen4);
		}
    }
}
