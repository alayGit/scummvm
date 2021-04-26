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
public ref class ManagedYEncoder: IByteEncoder, IMessageEncoder, ISaveDataEncoder
	{
	public:
		virtual System::String^ ByteEncode(cli::array<System::Byte>^ input);
		virtual cli::array<System::Byte>^ ByteDecode(System::String^ input);
	    virtual property Encoding^ TextEncoding { System::Text::Encoding^ get(); }


		ManagedYEncoder(ManagedCommon::Interfaces::ILogger ^ logger, ManagedCommon::Enums::Logging::LoggingCategory category);
	private:
	    ILogger ^ _logger;
	    LoggingCategory _category;
		Crc32 GetEmptyCrc();
	};
}
