#include "../../ExternalLibraries/include/7z/LzmaLib.h"
#include <assert.h>
#include <string>
#pragma once
typedef unsigned char byte;

namespace SevenZCompression {
byte* Compress(const byte* inBuf, size_t inBufLength, size_t &outBufLength, unsigned int level);
byte* Uncompress(const byte *inBuf, size_t inBufLength, size_t &outBufLength);
size_t GetUncompressedSize(const byte *compressed);
} // namespace SevenZCompression
