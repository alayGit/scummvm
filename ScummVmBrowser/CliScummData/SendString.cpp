#include "SendString.h";

CliScummEvents::SendString::SendString(String^ stringToSend)
{
	_stringToSend = stringToSend;
	queue = new std::queue<Common::Event*>();
	for (int i = 0; i < stringToSend->Length; i++)
	{
		Common::Event* event = new Common::Event;

		Char characterToSend = stringToSend[i];
		/*if (Char::IsUpper(toSend, i))
		{
			characterToSend = Char::ToLower(characterToSend);
			event->kbd.flags = Common::KBD_SHIFT;
		}*/
	
		event->type = Common::EventType::EVENT_KEYDOWN;
		
		event->kbd.ascii = Convert::ToUInt16(characterToSend);
		event->kbd.keycode = static_cast<Common::KeyCode>(characterToSend);

		queue->push(event);

		Common::Event* keyUpEvent = new Common::Event();
		keyUpEvent->type = Common::EventType::EVENT_KEYUP;
		keyUpEvent->kbd.ascii = event->kbd.ascii;
		keyUpEvent->kbd.keycode = event->kbd.keycode;

		queue->push(keyUpEvent);
	}
}

CliScummEvents::SendString::~SendString()
{
	delete queue;
}

String^ CliScummEvents::SendString::StringToSend::get() {
	return _stringToSend;
}

bool CliScummEvents::SendString::HasEvents()
{
	return !queue->empty();
}

System::IntPtr CliScummEvents::SendString::GetEvent()
{
	Common::Event* event = queue->front();

	queue->pop();

	return System::IntPtr(event);
}
