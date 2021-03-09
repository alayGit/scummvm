using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ManagedCommon.Delegates;
using ManagedCommon.Serializers;

namespace MessageBuffering
{
	public class ProcessMessages : IProcessMessageBuffers
	{
		AsyncQueue<Message> _messageQueue;
		Task _processTask;
		bool _stopped = false;
		IByteEncoder _byteEncoder;

		public ProcessMessages(IConfigurationStore<Enum> configurationStore, IByteEncoder byteEncoder, ILogger logger, IMessageCompression messageCompression)
		{
			_messageQueue = new AsyncQueue<Message>();
			_byteEncoder = byteEncoder;
	
			_processTask = Task.Run(async () =>
			{
				while (!_stopped || _messageQueue.Count != 0)
				{
					List<Message> dataList = new List<Message>();
					while (!_messageQueue.IsEmpty)
					{
						dataList.Add(await _messageQueue.DequeueAsync());
					}
					if (dataList.Count != 0 && MessagesProcessed != null)
					{
						string serialized = JsonConvert.SerializeObject(MergeLists(dataList));
						byte[] serializedCompressed = messageCompression.Compress(Encoding.ASCII.GetBytes(serialized));

						await MessagesProcessed(byteEncoder.AssciiByteEncode(serializedCompressed));
					}
					await Task.Delay(configurationStore.GetValue<int>(ScummHubSettings.BufferAndProcessSleepTime));
				}
			});
		}

		public MessagesProcessed MessagesProcessed { get; set; }

		private List<KeyValuePair<MessageType, string>> MergeLists(IEnumerable<Message> listToMerge)
		{
			JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
			serializerSettings.Converters.Add(new ByteArrayConverter(_byteEncoder));

			Dictionary<MessageType, List<Message>> messageTypeDictionary = new Dictionary<MessageType, List<Message>>();

			foreach (Message message in listToMerge)
			{
				if (!messageTypeDictionary.ContainsKey(message.MessageType))
				{
					messageTypeDictionary.Add(message.MessageType, new List<Message>());
				}

				messageTypeDictionary[message.MessageType].Add(message);
			}

			return messageTypeDictionary.Select(kvp => new KeyValuePair<MessageType, string>(kvp.Key, JsonConvert.SerializeObject(kvp.Value.Select(m => m.MessageContents), serializerSettings))).ToList();
		}


		public void Enqueue(object message, MessageType messageType)
		{
			if (!_stopped)
			{
				_messageQueue.Enqueue(new Message() { MessageType = messageType, MessageContents = message });
			}
		}

		public async Task Stop()
		{
			_stopped = true;

			await _processTask;
		}
	}
}
