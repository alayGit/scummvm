#include "SendMouseClick.h"

CliScummEvents::SendMouseClick::SendMouseClick(ManagedCommon::Enums::Actions::MouseClick clickEventType, GetCurrentMousePosition ^ getCurrentMousePosition, ManagedCommon::Enums::Other::MouseUpDown mouseUpDown) {
	_noTimesEventsDispatched = 0;
	_clickEventType = clickEventType;
	_getCurrentMousePosition = getCurrentMousePosition;
	_mouseUpDown = mouseUpDown;
}

bool CliScummEvents::SendMouseClick::HasEvents()
{
	return _noTimesEventsDispatched == 0;
}

System::IntPtr CliScummEvents::SendMouseClick::GetEvent()
{
	Common::Event* result = new Common::Event();  //Does not need to be destroyed, unless there is an error because ScummVM deals with it for us :)
	switch (_mouseUpDown)
	{
	case ManagedCommon::Enums::Other::MouseUpDown::MouseDown:
		switch (_clickEventType)
		{
		case MouseClick::Left:
			result->type = Common::EventType::EVENT_LBUTTONDOWN;
			break;
		case MouseClick::Middle:
			result->type = Common::EventType::EVENT_MBUTTONDOWN;
			break;
		case MouseClick::Right:
			result->type = Common::EventType::EVENT_RBUTTONDOWN;
			break;
		}
		break;
	case ManagedCommon::Enums::Other::MouseUpDown::MouseUp:
		switch (_clickEventType)
		{
		case MouseClick::Left:
			result->type = Common::EventType::EVENT_LBUTTONUP;
			break;
		case MouseClick::Middle:
			result->type = Common::EventType::EVENT_MBUTTONUP;
			break;
		case MouseClick::Right:
			result->type = Common::EventType::EVENT_RBUTTONUP;
			break;
		}
		break;
	default:
		delete result;
		throw gcnew Exception("Send mouse click somehow managed to get to " + _noTimesEventsDispatched + " events.");
	}

	_noTimesEventsDispatched++;

	System::Drawing::Point point = _getCurrentMousePosition();
	result->mouse.x = point.X;
	result->mouse.y = point.Y;

	return (System::IntPtr) result;
}

