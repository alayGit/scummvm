#pragma once
#include "../../common/str.h"
#pragma make_public(Common::String)
using namespace System::Runtime::InteropServices;

namespace ScummToManagedMarshalling {
	public ref class Converters
	{
	    public:
	        Common::String ManagedStringToCommonString(System::String ^ input);
	        System::String^ CommonStringToManagedString(Common::String input);
	};
}
