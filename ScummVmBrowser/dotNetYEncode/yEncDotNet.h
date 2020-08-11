#pragma once
#include "../yEncoder/yEncoder.h";
#include <string.h>;
#include <vector>;

using namespace ManagedCommon::Interfaces;
using namespace ManagedCommon::Enums::Logging;
using namespace ManagedCommon::Exceptions;
using namespace System::Runtime::InteropServices;
using namespace System::Text;
namespace yEncDotNet {
	public ref class YEncDotNet : IByteEncoder
	{
	public:
		YEncDotNet(ILogger^ logger, LoggingCategory category);
		virtual System::String^ AssciiEncode(cli::array<System::Byte>^ input);
		virtual cli::array<System::Byte>^ AssciiDecode(System::String^ input);
	private:
		Crc32 GetEmptyCrc();
		ILogger^ _logger;
		LoggingCategory _category;
	};
}