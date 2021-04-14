#pragma once
#include "../../common/str.h"
#include <vector>
#pragma make_public(Common::String)
using namespace System::Runtime::InteropServices;

namespace ScummToManagedMarshalling {
public
class Converters {
public:
	static Common::String ManagedStringToCommonString(System::String ^ input);
	static System::String ^ CommonStringToManagedString(const Common::String *input);

	template<typename T>
	    static cli::array<T> ^ MarshalVectorToManagedArray(std::vector<T> *input);

	template<typename T>
	static std::vector<T> *MarshalManagedArrayToVector(static cli::array<T> ^ input);
};
template<typename T>
    inline cli::array<T> ^ Converters::MarshalVectorToManagedArray(std::vector<T> *input) {
	cli::array<T> ^ result = gcnew cli::array<T>(input -> size());

	Marshal::Copy(System::IntPtr(&input->at(0)), result, 0, input->size());

	return result;
}
template<typename T>
inline std::vector<T> *Converters::MarshalManagedArrayToVector(cli::array<T> ^ input) {
	std::vector<T> *result = new std::vector<T>();
	result->resize(input->Length);

	Marshal::Copy(input, 0, System::IntPtr(&result->at(0)), input->Length);

	return result;
}
} // namespace ScummToManagedMarshalling
