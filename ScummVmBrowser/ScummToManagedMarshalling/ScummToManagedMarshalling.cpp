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

//template<typename T>
//    cli::array<T> ^ ScummToManagedMarshalling::Converters::MarshalVectorToManagedArray(std::vector<T> *input) {
//	cli::array<T> ^ result = gcnew cli::array<T>();
//
//	Marshal::Copy(System::IntPtr(&input->at(0)), result, 0, input->size());
//
//	return result;
//}
//
//template<typename T>
//std::vector<T> *ScummToManagedMarshalling::Converters::MarshalManagedArrayToVector(cli::array<T> ^ input) {
//	std::vector<T> *result = new std::vector<T>();
//	result->resize(input->Length);
//
//	Marshal::Copy(input, 0, System::IntPtr(&result->at(0)), input->size());
//
//	return result;
//}

