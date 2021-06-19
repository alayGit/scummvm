using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Enums.Other
{
	public enum InputType //Note must be in same order as InputMessageType in GameFrame.tsx
	{
        TextString,
		MouseDown,
		MouseUp,
		MouseMove,
		ControlKey
	}
}
