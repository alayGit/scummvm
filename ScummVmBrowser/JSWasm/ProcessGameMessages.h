#pragma once
#include <string>
#include <vector>
#include "../7ZCompression/7ZCompression.h"
#include "../yEncoder/yEncoder.h"
typedef unsigned char byte;
namespace JSWasm {
std::string InflateAndDecodeGameMessage(byte *deflatedAndEncoded, int deflatedAndEncodedLength);
};
