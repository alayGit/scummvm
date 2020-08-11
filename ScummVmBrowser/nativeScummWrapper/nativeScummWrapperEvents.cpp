#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "nativeScummVMWrapperEvents.h"


NativeScummWrapper::NativeScummWrapperEvents::NativeScummWrapperEvents(NativeScummWrapper::f_PollEvent pollEventWrapper, NativeScummWrapper::f_MoveMouse moveMouse) {
	NativeScummWrapperEvents::pollEventWrapper = pollEventWrapper;
	_moveMouse = moveMouse;
}

bool NativeScummWrapper::NativeScummWrapperEvents::pollEvent(Common::Event &event) {
	bool result =  pollEventWrapper(event);

	if (event.type == Common::EventType::EVENT_MOUSEMOVE)
	{
		_moveMouse(event.mouse.x, event.mouse.y);
	}

	return result;
}
