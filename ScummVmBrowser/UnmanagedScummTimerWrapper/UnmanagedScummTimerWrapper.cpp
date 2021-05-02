#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "UnmanagedScummTimerManagerWrapper.h"

UnmanagedScummTimerWrapper::UnmanagedScummTimerManagerWrapper::UnmanagedScummTimerManagerWrapper(ManagedCommon::Interfaces::IScummTimer ^ scummTimer) {
	_scummTimer = gcroot<ManagedCommon::Interfaces::IScummTimer^>(scummTimer);
}

bool UnmanagedScummTimerWrapper::UnmanagedScummTimerManagerWrapper::installTimerProc(TimerProc proc, int32 intervalMicroseconds, void *refCon, const Common::String &id) {
	return _scummTimer->InstallTimerProc(Marshal::GetDelegateForFunctionPointer<ScummTimerCallback ^>(System::IntPtr(proc)), intervalMicroseconds, System::IntPtr(refCon), ScummToManagedMarshalling::Converters::CommonStringToManagedString(&id));
}

void UnmanagedScummTimerWrapper::UnmanagedScummTimerManagerWrapper::removeTimerProc(TimerProc proc) {
	_scummTimer->RemoveTimerProc(Marshal::GetDelegateForFunctionPointer<ScummTimerCallback ^>(System::IntPtr(proc)));
}
