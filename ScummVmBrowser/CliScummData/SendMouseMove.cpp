#include "SendMouseMove.h"

CliScummEvents::SendMouseMove::SendMouseMove(int x, int y)
{
	_mouseMoveEvent = new Common::Event();
	
	_mouseMoveEvent->mouse.x = x;
	_mouseMoveEvent->mouse.y = y;
	_mouseMoveEvent->type = Common::EventType::EVENT_MOUSEMOVE;

	_spent = false;
}

CliScummEvents::SendMouseMove::~SendMouseMove()
{
	if (!_spent)
	{
		delete _mouseMoveEvent; //ScummVM will delete the event if it actually receives it
	}
}

bool CliScummEvents::SendMouseMove::HasEvents()
{
	return !_spent;
}

System::IntPtr CliScummEvents::SendMouseMove::GetEvent()
{
	if (_spent)
	{
		throw "The mouse move event was already spent";
	}
	_spent = true;

	return (System::IntPtr) _mouseMoveEvent;
}

