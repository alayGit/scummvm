using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	public enum MessageTypeEnum //Never used zero as it is a reserved character in the protocol
	{
		OnFrameReceived = 1, //List<ScreenBuffer>
		OnPlaySound = 2, //byte[]
		EnqueueControlKey = 3, //An int mapping to ControlKeys Enum
		EnqueueMouseClick = 4, //An int mapping to MouseClick Enum
		EnqueueMouseMove = 5,  //An array of two elements and x and a y
		EnqueueString = 6, //A string
		ScheduleRedrawWholeScreen = 7, //No message contents
		StartSound, //No message contents 
		StopSound //No message contents
	}
}
