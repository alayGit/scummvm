#pragma once
#include <string.h>;
#include <vector>;
#include <yEncoder.h>;

using namespace System::Runtime::InteropServices;
using namespace System::Text;
using namespace ManagedCommon::Interfaces;
using namespace ManagedCommon::Enums::Logging;
using namespace ManagedCommon::Exceptions;

namespace ManagedYEncoder {
public ref class ManagedYEncoder: IByteEncoder
	{
	public:
		virtual System::String^ AssciiByteEncode(cli::array<System::Byte>^ input);
		virtual cli::array<System::Byte>^ AssciiByteDecode(System::String^ input);
	    virtual System::String ^ AssciiStringEncode(System::String^ input);
	    virtual System::String ^ AssciiStringDecode(System::String ^ input);

		ManagedYEncoder(ManagedCommon::Interfaces::ILogger ^ logger, ManagedCommon::Enums::Logging::LoggingCategory category);
	private:
	    ILogger ^ _logger;
	    LoggingCategory _category;
		Crc32 GetEmptyCrc();
	};
}
