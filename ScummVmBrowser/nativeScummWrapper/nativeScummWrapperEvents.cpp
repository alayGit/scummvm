#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "nativeScummVMWrapperEvents.h"


NativeScummWrapper::NativeScummWrapperEvents::NativeScummWrapperEvents(NativeScummWrapper::f_MoveMouse moveMouse) {
	_moveMouse = moveMouse;

	_addEventToQueueMutex = CreateSemaphore( //Cannot use 'std::mutex due to an iteration issue with CLR
	    NULL,                          // default security attributes
	    1,                             // initial count
	    1,                             // maximum count
	    NULL);                         // unnamed semaphore
}

bool NativeScummWrapper::NativeScummWrapperEvents::pollEvent(Common::Event &event) {
	bool result = false;

	WaitForSingleObject(_addEventToQueueMutex, INFINITE);

	if (!_eventQueue.empty()) {

		event = _eventQueue.front();
		_eventQueue.pop();

		ReleaseSemaphore(_addEventToQueueMutex, 1, NULL); //Release the semaphore as soon as possible

		if (event.type == Common::EventType::EVENT_MOUSEMOVE) {
			_moveMouse(event.mouse.x, event.mouse.y);
		}

		result = true;
	} else {
		ReleaseSemaphore(_addEventToQueueMutex, 1, NULL);
	}

	return result;
}

void NativeScummWrapper::NativeScummWrapperEvents::addEventsToQueue(Common::Event *event, int length) {
	WaitForSingleObject(_addEventToQueueMutex, INFINITE);

	for (int i = 0; i < length; i++)
	{
		_eventQueue.push(event[i]);
	}

	ReleaseSemaphore(_addEventToQueueMutex, 1, NULL);
}
