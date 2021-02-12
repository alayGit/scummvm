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
	internal class Message
	{
		internal MessageType MessageType { get; set; }

		internal object MessageContents { get; set; }
	}
}
