#include "SendQuit.h";


CliScummEvents::SendQuit::SendQuit()
{
	hasEventBeingDispatched = false;
}

System::IntPtr CliScummEvents::SendQuit::GetEvent()
{
	Common::Event* event = new Common::Event();

	event->type = Common::EventType::EVENT_QUIT;

	hasEventBeingDispatched = true;

	return System::IntPtr(event);
}

bool CliScummEvents::SendQuit::HasEvents()
{
	return !hasEventBeingDispatched;
}
