// Emscripten7ZCompression.cpp : Defines the functions for the static library.
//

#include "EmscriptenProcessGameMessages.h"


extern "C" {
EMSCRIPTEN_KEEPALIVE
byte *InflateAndDecodeGameMessage(byte *deflatedAndEncoded, size_t deflatedAndEncodedLength, size_t &uncompressedLength) {
	return JSWasm::InflateAndDecodeGameMessage(deflatedAndEncoded, deflatedAndEncodedLength, uncompressedLength);
}

EMSCRIPTEN_KEEPALIVE
byte* alloc(size_t size) {
	return new byte[size];
}

EMSCRIPTEN_KEEPALIVE
    void dealloc(byte* toFree) {
	return delete[] toFree;
}

}
