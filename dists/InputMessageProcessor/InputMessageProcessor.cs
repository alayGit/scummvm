using ManagedCommon.Interfaces;
using ManagedCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputMessageProcessor
{
	public class InputMessageProcessor : IInputMessageProcessor
	{
		public Task SendInputMessageMessage(InputMessage[] inputMessages, IGameInfo gameInfo)
		{
			throw new NotImplementedException();
		}
	}
}
