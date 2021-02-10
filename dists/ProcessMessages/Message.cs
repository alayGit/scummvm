using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MessageBuffering
{
	public class Message
	{
		public MessageType MessageType { get; set; }

		public object MessageContents { get; set; }
	}
}
