#pragma once
#include "scummVm.h"
#include <functional>
namespace NativeScummWrapper {
	typedef bool(__stdcall *f_PollEvent)(Common::Event &event);
	typedef std::function<void(int x, int y)> f_MoveMouse;
	class NativeScummWrapperEvents : public Common::EventSource {
	public:
		NativeScummWrapperEvents(f_PollEvent eventQueue, f_MoveMouse moveMouse);
		bool pollEvent(Common::Event &event) override;
		f_PollEvent pollEventWrapper;
		f_MoveMouse _moveMouse;
	};
} // namespace NativeScummWrapper
