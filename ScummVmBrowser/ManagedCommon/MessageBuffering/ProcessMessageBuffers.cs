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

namespace ManagedCommon.MessageBuffering
{
	public class ProcessMessageBuffers
	{
		AsyncQueue<IMessage<IEnumerable<object>>> _messageQueue;
		Task _processTask;
		bool _stopped = false;
		IByteEncoder _byteEncoder;

		public ProcessMessageBuffers(Func<List<KeyValuePair<MessageType, string>>, Task> processCallback, IConfigurationStore<Enum> configurationStore, IByteEncoder byteEncoder)
		{
			_messageQueue = new AsyncQueue<IMessage<IEnumerable<object>>>();
			_byteEncoder = byteEncoder;
			_processTask = Task.Run(async () =>
			{
				while (!_stopped)
				{
					List<IMessage<IEnumerable<object>>> dataList = new List<IMessage<IEnumerable<object>>>();
					while (!_messageQueue.IsEmpty)
					{
						dataList.Add(await _messageQueue.DequeueAsync());
					}
					if (dataList.Count != 0)
					{
						//await processCallback(dataList.GroupBy(x => x.MessageType).SelectMany(g => new List<IMessage<IEnumerable<object>>>() { new Message<IEnumerable<object>>() { MessageType = g.First().MessageType, MessageContents = g.SelectMany(m => m.MessageContents) } }));
						await processCallback(MergeLists(dataList));
					}
					await Task.Delay(configurationStore.GetValue<int>(ScummHubSettings.BufferAndProcessSleepTime));
				}
			});
		}


		private List<KeyValuePair<MessageType, string>> MergeLists(IEnumerable<IMessage<IEnumerable<object>>> listToMerge)
		{
			JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
			serializerSettings.Converters.Add(new ByteArrayConverter(_byteEncoder));

			Dictionary<MessageType, IMessage<List<object>>> messageTypeDictionary = new Dictionary<MessageType, IMessage<List<object>>>();

			foreach(IMessage<IEnumerable<object>> messages in listToMerge)
			{
				if(!messageTypeDictionary.ContainsKey(messages.MessageType))
				{
					messageTypeDictionary.Add(messages.MessageType, new Message<List<object>> { MessageType = messages.MessageType, MessageContents = new List<object>() });
				}

				messageTypeDictionary[messages.MessageType].MessageContents.AddRange(messages.MessageContents);
			}

			return messageTypeDictionary.Select(kvp => new KeyValuePair<MessageType,string>(kvp.Value.MessageType, JsonConvert.SerializeObject(kvp.Value.MessageContents, serializerSettings))).ToList();
		}


		public void Enqueue(IMessage<IEnumerable<object>> message)
		{
			_messageQueue.Enqueue(message);
		}

		public async Task Stop()
		{
			_stopped = true;

			await _processTask;
		}
	}
}
