#pragma once
#include "../../common/str.h"
#include <vector>
#pragma make_public(Common::String)
using namespace System::Runtime::InteropServices;
using namespace System::Text;

namespace ScummToManagedMarshalling {
public
class Converters {
public:
	static Common::String ManagedStringToCommonString(System::String ^ input);
	static System::String ^ CommonStringToManagedString(const Common::String *input);

	static byte *MarshalManagedStringToByteArray(System::String^ input, Encoding^ encoder);

	template<typename T>
	    static cli::array<T> ^ MarshalVectorToManagedArray(std::vector<T> *input, bool deleteVectorAfterUse = true);

	template<typename T>
	static std::vector<T> *MarshalManagedArrayToVector(static cli::array<T> ^ input);
};
template<typename T>
    inline cli::array<T> ^ Converters::MarshalVectorToManagedArray(std::vector<T> *input, bool deleteVectorAfterUse) {
	cli::array<T> ^ result = gcnew cli::array<T>(input -> size());

	Marshal::Copy(System::IntPtr(&input->at(0)), result, 0, input->size());

	if (deleteVectorAfterUse)
	{
		delete input;
	}

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
