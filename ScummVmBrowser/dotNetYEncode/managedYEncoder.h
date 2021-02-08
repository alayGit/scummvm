#pragma once
#include "../yEncoder/yEncoder.h";
#include <string.h>;
#include <vector>;

using namespace System::Runtime::InteropServices;
using namespace System::Text;
using namespace ManagedCommon::Interfaces;

namespace ManagedYEncoder {
public ref class ManagedYEncoder: IByteEncoder
	{
	public:
		virtual System::String^ AssciiByteEncode(cli::array<System::Byte>^ input);
		virtual cli::array<System::Byte>^ AssciiByteDecode(System::String^ input);
	    virtual System::String ^ AssciiStringEncode(System::String^ input);
	    virtual System::String ^ AssciiStringDecode(System::String ^ input);
	private:
		Crc32 GetEmptyCrc();
	};
}
