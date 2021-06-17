#pragma once
#pragma once
#pragma once
#include "../../common/events.h"
using namespace System::Collections::Generic;
using namespace System;
using namespace ManagedCommon::Interfaces;
using namespace  ManagedCommon::Enums::Actions;
using namespace  ManagedCommon::Delegates;
namespace CliScummEvents {
	public ref class SendMouseClick : IGameEvent {
	public:
	        SendMouseClick(ManagedCommon::Enums::Actions::MouseClick clickEventType, GetCurrentMousePosition ^ getCurrentMousePosition, ManagedCommon::Enums::Other::MouseUpDown mouseUpDown);
		virtual bool HasEvents() override;
		virtual System::IntPtr GetEvent() override;
	private:
	    ManagedCommon::Enums::Other::MouseUpDown _mouseUpDown;
		int _noTimesEventsDispatched;
		ManagedCommon::Enums::Actions::MouseClick _clickEventType;
		GetCurrentMousePosition^ _getCurrentMousePosition;
	};
}
