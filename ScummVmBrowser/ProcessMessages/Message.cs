using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using Newtonsoft.Json;
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
		[JsonProperty]
		internal MessageType MessageType { get; set; }

		[JsonProperty]
		internal object MessageContents { get; set; }
	}
}
