#pragma once
#include "ZLibCompression.h"
#include "string.h"
using namespace ManagedCommon::Interfaces;
using namespace System::Runtime::InteropServices;
//
typedef unsigned char byte;

namespace ManagedZLibCompression {
public
ref class ManagedZLibCompression : public ManagedCommon::Interfaces::IMessageCompression
	{
		public:
			virtual cli::array<System::Byte>^ Compress(cli::array<System::Byte>^ input);
			virtual cli::array<System::Byte>^ Decompress(cli::array<System::Byte>^ input);
	};
};
