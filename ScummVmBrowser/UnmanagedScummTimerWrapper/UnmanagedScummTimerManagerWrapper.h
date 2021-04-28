#pragma once

#include "../../common/timer.h"
#include "../../common/scummsys.h";
#include "../../common/str.h";
#include "../ScummToManagedMarshalling/ScummToManagedMarshalling.h";
#include <msclr/gcroot.h>

using namespace msclr;
using namespace System::Runtime::InteropServices;
using namespace ManagedCommon::Delegates;
using namespace ManagedCommon::Interfaces;

namespace UnmanagedScummTimerWrapper {
class UnmanagedScummTimerManagerWrapper : public Common::TimerManager
	{
public:
	    UnmanagedScummTimerManagerWrapper(IScummTimer^ scummTimer);
		virtual bool installTimerProc(TimerProc proc, int32 interval, void *refCon, const Common::String &id);
		virtual void removeTimerProc(TimerProc proc);

	private:
	    gcroot<IScummTimer ^> _scummTimer;
	};
}
