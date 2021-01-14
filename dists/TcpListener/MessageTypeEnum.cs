using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	public enum MessageTypeEnum //Never used zero as it is a reserved character in the protocol
	{
		OnFrameReceived = 1,
		OnPlaySound = 2,
	}
}
