#pragma once
#include "zlib.h"
#include "string.h"
using namespace ManagedCommon::Interfaces;
using namespace System::Runtime::InteropServices;
//
//typedef unsigned char byte;

namespace Compression {
	public ref class ZLibCompression: public ManagedCommon::Interfaces::ICompression
	{
		public:
			virtual cli::array<System::Byte>^ Compress(cli::array<System::Byte>^ input);
			virtual cli::array<System::Byte>^ Decompress(cli::array<System::Byte>^ input);
	};
};
