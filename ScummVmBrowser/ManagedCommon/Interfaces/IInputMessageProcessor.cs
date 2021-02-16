using ManagedCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
	public interface IInputMessageProcessor
	{
		Task SendInputMessageMessage(InputMessage[] inputMessages, IGameInfo gameInfo);
	}
}
