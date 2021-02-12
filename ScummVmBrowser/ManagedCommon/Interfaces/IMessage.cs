using ManagedCommon.Enums;
using System.Collections;

namespace ManagedCommon.Interfaces
{
	public interface IMessage<out T> where T: IEnumerable
	{
		MessageType MessageType { get; }
		T MessageContents { get; }
	}
}
