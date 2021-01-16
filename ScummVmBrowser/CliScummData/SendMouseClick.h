#pragma once
#pragma once
#pragma once
#include "../../common/events.h"
using namespace System::Collections::Generic;
using namespace System;
using namespace ManagedCommon::Interfaces;
using namespace  ManagedCommon::Enums;
using namespace  ManagedCommon::Delegates;
namespace CliScummEvents {
	public ref class SendMouseClick : IGameEvent {
	public:
		SendMouseClick(ManagedCommon::Enums::MouseClick clickEventType, GetCurrentMousePosition^ getCurrentMousePosition);
		virtual bool HasEvents() override;
		virtual System::IntPtr GetEvent() override;
	private:
		int _noTimesEventsDispatched;
		ManagedCommon::Enums::MouseClick _clickEventType;
		GetCurrentMousePosition^ _getCurrentMousePosition;
	};
}
