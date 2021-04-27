#include "pch.h"
#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "ScummToManagedMarshalling.h"

byte *ScummToManagedMarshalling::Converters::MarshalManagedStringToByteArray(System::String ^ input, Encoding^ encoder) {
	byte *result = new byte[input->Length];

	Marshal::Copy(encoder->GetBytes(input), 0, System::IntPtr(result), input->Length);

	return result;
}


Common::String ScummToManagedMarshalling::Converters::ManagedStringToCommonString(System::String ^ input) {
	char *p = (char *)Marshal::StringToHGlobalAnsi(input).ToPointer();
	Common::String result = Common::String(p);
	Marshal::FreeHGlobal(System::IntPtr(p));

	return result;
}

System::String ^ ScummToManagedMarshalling::Converters::CommonStringToManagedString(const Common::String *input) {
	return gcnew System::String(input->c_str());
}
