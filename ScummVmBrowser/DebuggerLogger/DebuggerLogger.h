#pragma once
#include <cstdarg>
#include <string>


namespace DebuggerTools {
	class DebuggerLogger
	{
	    public:
	        static void Log(const char *toLog, const char *fileName, const int noArgs, ...);

	    private:
	        static void Log(const char *toLog, const char *fileName, const int noArgs, const std::string* args);
	};
}
