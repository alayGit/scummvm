// UnmanagedHelpers.cpp : Defines the functions for the static library.
//
#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "UnmanagedHelpers.h"

uint32 UnmanagedHelpers::ScummByteStream::write(const void *dataPtr, uint32 dataSize) {
	for (int i = 0; i < dataSize; i++)
	{
		_stream.push_back(((byte*) dataPtr)[i]);
	}

	return dataSize;
}

std::vector<byte> UnmanagedHelpers::ScummByteStream::getStream() {
	return _stream;
}
