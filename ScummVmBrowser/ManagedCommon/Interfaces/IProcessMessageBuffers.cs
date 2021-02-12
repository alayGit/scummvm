using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
	public interface IProcessMessageBuffers
	{
		void Enqueue(object message, MessageType messageType);
		Task Stop();
		MessagesProcessed MessagesProcessed { get; set; }
	}
}
