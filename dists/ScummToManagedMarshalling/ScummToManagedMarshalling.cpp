#include "pch.h"

#include "ScummToManagedMarshalling.h"

Common::String ScummToManagedMarshalling::Converters::ManagedStringToCommonString(System::String ^ input) {
	char *p = (char *)Marshal::StringToHGlobalAnsi(input).ToPointer();
	Common::String result = Common::String(p);
	Marshal::FreeHGlobal(System::IntPtr(p));

	return result;
}

System::String^ ScummToManagedMarshalling::Converters::CommonStringToManagedString(Common::String input) {
	return gcnew System::String(input.c_str());
}
