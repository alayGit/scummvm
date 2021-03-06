#pragma once
#include "../7ZCompression/7ZCompression.h";
#include <vector>

using namespace System;
using namespace System::Runtime::InteropServices;

namespace SevenZCompression {
public
ref class SevenZCompressor : ManagedCommon::Interfaces::IMessageCompression, ManagedCommon::Interfaces::ISaveDataCompression
	{
	 public:
	    virtual cli::array<System::Byte>^ Compress(cli::array<System::Byte>^ data, unsigned int level);
	    virtual cli::array<System::Byte> ^ Decompress(cli::array<System::Byte> ^ data);
	};
}
