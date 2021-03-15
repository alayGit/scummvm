#include "7ZCompression.h"
const int COMPRESSED_SIZE = 4;

byte* SevenZCompression::Compress(byte *inBuf, size_t inBufLength, size_t &outBufLength) {
	size_t propsSize = LZMA_PROPS_SIZE;
	size_t dstLength = inBufLength + inBufLength / 3 + 128;

	byte *outBuf = new byte[dstLength + propsSize + COMPRESSED_SIZE];

	int res = LzmaCompress(
	    &outBuf[LZMA_PROPS_SIZE + COMPRESSED_SIZE], &dstLength,
	    inBuf, inBufLength,
	    outBuf, &propsSize,
	    -1, 0, -1, -1, -1, -1, -1);

	memcpy(&outBuf[LZMA_PROPS_SIZE], &inBufLength, COMPRESSED_SIZE);

	assert(propsSize == LZMA_PROPS_SIZE);
	assert(res == SZ_OK);

	byte *result = new byte[dstLength + propsSize];
	memcpy(result, outBuf, dstLength + propsSize);
	outBufLength = dstLength + propsSize;


	delete[] outBuf;

	return result;
}

byte* SevenZCompression::Uncompress(byte *inBuf, size_t inBufLength, size_t &outBufLength)
{
	outBufLength = GetUncompressedSize(inBuf);
	size_t srcLen = inBufLength - LZMA_PROPS_SIZE;

	byte *outBuf = new byte[outBufLength];

	SRes res = LzmaUncompress(
	    outBuf, (size_t *) &outBufLength,
	    &inBuf[LZMA_PROPS_SIZE + COMPRESSED_SIZE], &srcLen,
	    inBuf, LZMA_PROPS_SIZE);
	assert(res == SZ_OK);

	byte *result = new byte[outBufLength];
	memcpy(result, outBuf, outBufLength);

	delete[] outBuf;

	return result;
}

size_t SevenZCompression::GetUncompressedSize(byte *compressed) {
	size_t result = 0;

	memcpy(&result, compressed + LZMA_PROPS_SIZE, COMPRESSED_SIZE);

	return result;
}
