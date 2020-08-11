
using System;

namespace ManagedCommon.Interfaces {
	public interface IGameEvent
	{
		bool HasEvents();
		IntPtr GetEvent();
	};
}

