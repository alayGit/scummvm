#include "../../ExternalLibraries/include/7z/LzmaLib.h"
#include <assert.h>
#include <string>
#pragma once
typedef unsigned char byte;

namespace SevenZCompression {
byte* Compress(byte* inBuf, size_t inBufLength, size_t &outBufLength);
byte* Uncompress(byte *inBuf, size_t inBufLength, size_t &outBufLength);
} // namespace SevenZCompression
