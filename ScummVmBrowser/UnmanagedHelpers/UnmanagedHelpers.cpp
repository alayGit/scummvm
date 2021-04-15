// UnmanagedHelpers.cpp : Defines the functions for the static library.
//
#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "UnmanagedHelpers.h"

UnmanagedHelpers::ScummByteStream::ScummByteStream() {
	_data = new std::vector<byte>();
}

UnmanagedHelpers::ScummByteStream::~ScummByteStream() {
	delete _data;
}

uint32 UnmanagedHelpers::ScummByteStream::write(const void *dataPtr, uint32 dataSize) {
	for (int i = 0; i < dataSize; i++)
	{
		_data->push_back(((byte*) dataPtr)[i]);
	}

	return dataSize;
}

int32 UnmanagedHelpers::ScummByteStream::pos() const {
	return _data->size();
}


std::vector<byte>* UnmanagedHelpers::ScummByteStream::getData() {
	return new std::vector<byte>(*_data);
}
