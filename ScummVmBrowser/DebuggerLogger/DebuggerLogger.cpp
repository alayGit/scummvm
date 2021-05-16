#include "DebuggerLogger.h"
#include "../LaunchDebugger/LaunchDebugger.h"

void DebuggerTools::DebuggerLogger::Log(const char *toLog, const char *fileName, const int noArgs, ...) {
	va_list vaArgs;
	va_start(vaArgs, noArgs);

	std::string *args = new std::string[noArgs];

	for (int i = 0; i < noArgs; i++)
	{
		args[i] = std::to_string(va_arg(vaArgs, int));
	}
	va_end(vaArgs);

	Log(toLog,fileName, noArgs, args);

	delete[] args;
}

void DebuggerTools::DebuggerLogger::Log(const char *toLog, const char *fileName, const int noArgs, const std::string *args) {
	cli::array<System::String ^> ^ managedArgs = gcnew cli::array<System::String ^>(noArgs);

	for (int i = 0; i < noArgs; i++) {
		managedArgs[i] = gcnew System::String(args[i].c_str());
	}

	System::String ^ managedToLog = gcnew System::String(toLog);
	System::String ^ managedFileName = gcnew System::String(fileName);
	System::String ^ formattedData = System::String::Format(managedToLog, managedArgs);

	System::IO::File::AppendAllText(managedFileName, formattedData);
}
