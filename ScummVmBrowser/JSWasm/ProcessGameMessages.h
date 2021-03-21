#pragma once
#include <string>
#include <vector>
#include "../7ZCompression/7ZCompression.h"
#include "../yEncoder/yEncoder.h"
typedef unsigned char byte;
namespace JSWasm {
byte *InflateAndDecodeGameMessage(byte *deflatedAndEncoded, int deflatedAndEncodedLength, size_t &uncompressedLength);
};
