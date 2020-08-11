#pragma once


#include "../../common/events.h"
using namespace System;
using namespace System::Collections::Generic;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;
namespace EventTests {
	[TestClassAttribute]
	public ref class EventTests
	{
	public:
		[TestMethodAttribute]
		void SendQuitSendsQuit();

		[TestMethodAttribute]
		void SendStringSendsCorrectString();
		
		[TestMethodAttribute]
		void SendStringControlCharactersSendsCorrectChar();
	};
}
