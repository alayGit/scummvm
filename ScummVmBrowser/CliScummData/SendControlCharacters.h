#pragma once
#include "../../common/events.h"
using namespace System::Collections::Generic;
using namespace System;
using namespace ManagedCommon::Interfaces;
using namespace ManagedCommon::Enums;
namespace CliScummEvents {
	public ref class SendControlCharacters : IGameEvent {
	public:
		SendControlCharacters(ControlKeys controlKey);
		virtual bool HasEvents() override;
		virtual System::IntPtr GetEvent() override;

		property ControlKeys ControlKey {
			ControlKeys get();
		}

	private:
		ControlKeys controlKey;
		Common::Event* eventList;
		int counter;

		void CreateEvents();
	};
}
