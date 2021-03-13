#include "../../ExternalLibraries/include/7z/LzmaLib.h";
#include <vector>
#include <assert.h>
#pragma once
typedef unsigned char byte;

namespace SevenZCompression {
void Compress(std::vector<unsigned char> &outBuf, const std::vector<unsigned char> &inBuf);
void Uncompress(std::vector<unsigned char> &outBuf, const std::vector<unsigned char> &inBuf);
} // namespace SevenZCompression
