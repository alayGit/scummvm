#include "SendControlCharacters.h"

CliScummEvents::SendControlCharacters::SendControlCharacters(ControlKeys controlKey)
{
	SendControlCharacters::controlKey = controlKey;
	CreateEvents();
	int counter = 0;
}

ControlKeys CliScummEvents::SendControlCharacters::ControlKey::get() {
	return controlKey;
}

bool CliScummEvents::SendControlCharacters::HasEvents()
{
	return counter < 2;
}

System::IntPtr CliScummEvents::SendControlCharacters::GetEvent()
{
	return System::IntPtr(&eventList[counter++]);
}

void CliScummEvents::SendControlCharacters::CreateEvents()
{
	 eventList = new Common::Event[2];

	eventList[0] = *(new Common::Event());
	eventList[0].type = Common::EventType::EVENT_KEYDOWN;
	switch (controlKey)
	{
	case ControlKeys::Delete:
		eventList[0].kbd.keycode = Common::KEYCODE_DELETE;
		break;
	case ControlKeys::Escape:
		eventList[0].kbd.ascii = Common::KEYCODE_ESCAPE;
		eventList[0].kbd.keycode = Common::KEYCODE_ESCAPE;
		break;
	case ControlKeys::F1:
		eventList[0].kbd.keycode = Common::KEYCODE_F1;
		break;
	case ControlKeys::F2:
		eventList[0].kbd.keycode = Common::KEYCODE_F2;
		break;
	case ControlKeys::F3:
		eventList[0].kbd.keycode = Common::KEYCODE_F3;
		break;
	case ControlKeys::F4:
		eventList[0].kbd.keycode = Common::KEYCODE_F4;
		break;
	case ControlKeys::F5:
		eventList[0].kbd.keycode = Common::KEYCODE_F5;
		break;
	case ControlKeys::F6:
		eventList[0].kbd.keycode = Common::KEYCODE_F6;
		break;
	case ControlKeys::F7:
		eventList[0].kbd.keycode = Common::KEYCODE_F7;
		break;
	case ControlKeys::F8:
		eventList[0].kbd.keycode = Common::KEYCODE_F8;
		break;
	case ControlKeys::F9:
		eventList[0].kbd.keycode = Common::KEYCODE_F8;
		break;
	case ControlKeys::F10:
		eventList[0].kbd.keycode = Common::KEYCODE_F10;
		break;
	case ControlKeys::F11:
		eventList[0].kbd.keycode = Common::KEYCODE_F11;
		break;
	case ControlKeys::F12:
		eventList[0].kbd.keycode = Common::KEYCODE_F12;
		break;
	case ControlKeys::ArrowUp:
		eventList[0].kbd.keycode = Common::KEYCODE_UP;
		break;
	case ControlKeys::ArrowDown:
		eventList[0].kbd.keycode = Common::KEYCODE_DOWN;
		break;
	case ControlKeys::ArrowRight:
		eventList[0].kbd.keycode = Common::KEYCODE_RIGHT;
		break;
	case ControlKeys::ArrowLeft:
		eventList[0].kbd.keycode = Common::KEYCODE_LEFT;
		break;
	case ControlKeys::Tab:
		eventList[0].kbd.keycode = Common::KeyCode::KEYCODE_TAB;
		eventList[0].kbd.ascii = Common::KeyCode::KEYCODE_TAB;
		break;
	case ControlKeys::Backspace:
		eventList[0].kbd.keycode = Common::KeyCode::KEYCODE_BACKSPACE;
		eventList[0].kbd.ascii = Common::KeyCode::KEYCODE_BACKSPACE;
		break;
	}
   
	eventList[1] = *(new Common::Event());
	eventList[1].kbd.keycode = eventList[0].kbd.keycode;
	eventList[1].kbd.ascii = eventList[0].kbd.ascii;
	eventList[1].type = Common::EventType::EVENT_KEYUP;
}
