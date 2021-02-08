using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.MessageBuffering
{
	public class Message<T>  : IMessage<T> where T : IEnumerable
	{
		public MessageType MessageType { get; set; }

		public T MessageContents { get; set; }
	}
}
