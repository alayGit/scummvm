#pragma once

#include "windows.h"
#include <processthreadsapi.h>
#include <sstream>
#include <string>;
#include <sysinfoapi.h>;


namespace DebuggerTools {
class DebuggerLauncher {
private:
	static bool isDebuggerLaunched;

public:
	bool launchDebugger();
};
} // namespace DebuggerTools
