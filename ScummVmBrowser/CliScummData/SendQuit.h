#pragma once
#include "../../common/events.h"
using namespace ManagedCommon::Interfaces;

namespace CliScummEvents {
	public ref class SendQuit : IGameEvent {
	public:
		SendQuit();
		virtual System::IntPtr GetEvent() override;
		virtual bool HasEvents() override;
	private:
		bool hasEventBeingDispatched;
	};
};