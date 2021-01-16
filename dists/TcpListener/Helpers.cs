using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	internal class Helpers
	{
		internal static byte[] ConvertObjectToMessage(object toSerialize, MessageTypeEnum messageTypeEnum)
		{
			string serialized = JsonConvert.SerializeObject(toSerialize);

			byte[] asciiSerializedBytes = Encoding.ASCII.GetBytes(serialized);
			byte[] asciiSerializedBytesWithMessage = new byte[asciiSerializedBytes.Length + 1];
			asciiSerializedBytesWithMessage[0] = (byte)messageTypeEnum;
			Array.Copy(asciiSerializedBytes, 0, asciiSerializedBytesWithMessage, 1, asciiSerializedBytes.Length);

			return asciiSerializedBytesWithMessage;
		}

		internal static byte[] CreateMessageWithNoContents(MessageTypeEnum messageTypeEnum)
		{
			return new byte[] { (byte)messageTypeEnum };
		}

		internal static string GetMessage(byte[] value, out MessageTypeEnum messageType)
		{
			if (value.Count() < 1)
			{
				throw new Exception("Fail: No message data");

			}

			byte[] messagePacket = value.Skip(1).ToArray();
			messageType = (MessageTypeEnum)value[0];

			string messageJson = string.Empty;
			if (messagePacket.Count() > 0)
			{
				messageJson = Encoding.ASCII.GetString(messagePacket);
			}

			return messageJson;
		}
	}
}
