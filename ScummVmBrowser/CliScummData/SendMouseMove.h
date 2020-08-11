#pragma once
#pragma once
#include "../../common/events.h"
using namespace System::Collections::Generic;
using namespace System;
using namespace ManagedCommon::Interfaces;
namespace CliScummEvents {
	public ref class SendMouseMove : IGameEvent {
	public:
		CliScummEvents::SendMouseMove(int x, int y);
		~SendMouseMove();
		virtual bool HasEvents() override;
		virtual System::IntPtr GetEvent() override;		
	private:
		Common::Event* _mouseMoveEvent;
		bool _spent;
	};
}