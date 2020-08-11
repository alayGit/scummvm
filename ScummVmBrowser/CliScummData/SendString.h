#pragma once
#include <queue> 
#include "../../common/keyboard.h"
#include "../../common/events.h"

using namespace System;
using namespace System::Collections::Concurrent;
using namespace ManagedCommon::Interfaces;

namespace CliScummEvents {
	public ref class SendString : IGameEvent {
	public:
		SendString(String^ toSend);
		~SendString();
		virtual bool HasEvents() override;
		virtual System::IntPtr GetEvent() override;

		property String^ StringToSend {
			String^ get();
		}

	private:
		std::queue<Common::Event*>* queue;
		String^ _stringToSend;
	};
}