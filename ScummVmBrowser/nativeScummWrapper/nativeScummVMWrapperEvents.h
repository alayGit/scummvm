#pragma once
#include "scummVm.h"
#include <functional>
#include <queue>
#include <windows.h>

namespace NativeScummWrapper {
	typedef std::function<void(int x, int y)> f_MoveMouse;
	class NativeScummWrapperEvents : public Common::EventSource {
	public:
		NativeScummWrapperEvents(f_MoveMouse moveMouse);
		bool pollEvent(Common::Event &event) override;
	    void addEventsToQueue(Common::Event* event, int length);
		f_MoveMouse _moveMouse;
	    HANDLE _addEventToQueueMutex;

	private:
	    std::queue<Common::Event> _eventQueue;
	};
} // namespace NativeScummWrapper
