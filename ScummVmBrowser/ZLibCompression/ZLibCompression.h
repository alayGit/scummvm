#pragma once
#include "string.h"
#include "c:\\ScummLib\\include\\zlib.h"
//
typedef unsigned char byte;

namespace ZLibCompression {
class ZLibCompression {
public:
	byte* Compress(byte* input, int inputLength, int& outputLength);
	byte* Decompress(byte* input, int inputLength, int& outputLength);
};
}; // namespace Compression
