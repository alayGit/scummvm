// Emscripten7ZCompression.cpp : Defines the functions for the static library.
//

#include "Emscripten7ZCompression.h"

extern "C" {
	EMSCRIPTEN_KEEPALIVE
	byte *Compress(byte *inBuf, size_t inBufLength, size_t &outBufLength) {
		return SevenZCompression::Compress(inBuf, inBufLength, outBufLength);
	}
	EMSCRIPTEN_KEEPALIVE
	byte *Uncompress(byte *inBuf, size_t inBufLength, size_t &outBufLength) {
		return SevenZCompression::Uncompress(inBuf, inBufLength, outBufLength);
	}
}


