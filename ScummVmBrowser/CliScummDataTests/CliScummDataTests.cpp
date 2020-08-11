#include "pch.h"

#include "CliScummDataTests.h"

using namespace ManagedCommon::Enums;
using namespace CliScummEvents;

void EventTests::EventTests::SendStringControlCharactersSendsCorrectChar()
{
	Common::KeyCode TestChar = Common::KeyCode::KEYCODE_TAB;
	SendControlCharacters^ sendCtrlChar = gcnew SendControlCharacters(ControlKeys::Tab);
		
	for (int i = 0; i < 2; i++)
	{
		Assert::IsTrue(sendCtrlChar->HasEvents());

		Common::Event* event = (Common::Event*) sendCtrlChar->GetEvent().ToPointer();

		if (i == 0)
		{
			Assert::AreEqual((int)event->type, (int)Common::EventType::EVENT_KEYDOWN);
		}
		else
		{
			Assert::AreEqual((int)event->type, (int)Common::EventType::EVENT_KEYUP);
		}

		Assert::AreEqual((int)event->kbd.ascii, (int)TestChar);
		Assert::AreEqual((int)event->kbd.keycode,(int)TestChar);
	}

	Assert::IsFalse(sendCtrlChar->HasEvents());
}


void EventTests::EventTests::SendQuitSendsQuit()
{
	SendQuit^ sendQuit = gcnew SendQuit();
	
	Assert::IsTrue(sendQuit->HasEvents());
	Common::Event* event = (Common::Event*) sendQuit->GetEvent().ToPointer();
	
	Assert::IsFalse(sendQuit->HasEvents());
	Assert::AreEqual((int) Common::EventType::EVENT_QUIT,(int) event->type);
}

void EventTests::EventTests::SendStringSendsCorrectString()
{
	String^ TestString = "abc";
	array<wchar_t>^ testStringArray = TestString->ToCharArray();
	SendString^ sendString = gcnew SendString(TestString);

	for each (wchar_t c in testStringArray)
	{
		for (int i = 0; i < 2; i++)
		{
			Assert::IsTrue(sendString->HasEvents());
			Common::Event* event = (Common::Event*) sendString->GetEvent().ToPointer();

			if (i == 0)
			{
				Assert::AreEqual((int) event->type, (int) Common::EventType::EVENT_KEYDOWN);
			}
			else
			{
				Assert::AreEqual((int)event->type, (int)Common::EventType::EVENT_KEYUP);
			}

			Assert::AreEqual((wchar_t) event->kbd.ascii, c);
			Assert::AreEqual((wchar_t) event->kbd.keycode, c);
		}
	}
	Assert::IsFalse(sendString->HasEvents());
}


