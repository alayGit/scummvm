using ManagedCommon.Delegates;
using ManagedCommon.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
	public interface IProcessMessageBuffers
	{
		void Enqueue(IMessage<IEnumerable<object>> message);
		Task Stop();
		MessagesProcessed MessagesProcessed { get; set; }
	}
}
