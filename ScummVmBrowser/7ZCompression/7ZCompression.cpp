#include "7ZCompression.h"
const int COMPRESSED_SIZE = 4;
const int MAX_LEVEL = 10;

byte* SevenZCompression::Compress(const byte *inBuf, size_t inBufLength, size_t &outBufLength, unsigned int level) {
	size_t propsSize = LZMA_PROPS_SIZE;
	size_t dstLength = inBufLength + inBufLength / 3 + 128;

	byte *outBuf = new byte[dstLength + propsSize + COMPRESSED_SIZE];

	int res = LzmaCompress(
	    &outBuf[LZMA_PROPS_SIZE + COMPRESSED_SIZE], &dstLength,
	    inBuf, inBufLength,
	    outBuf, &propsSize,
	    level <= MAX_LEVEL ? level : MAX_LEVEL, 0, -1, -1, -1, -1, -1);

	memcpy(&outBuf[LZMA_PROPS_SIZE], &inBufLength, COMPRESSED_SIZE);

	assert(propsSize == LZMA_PROPS_SIZE);
	assert(res == SZ_OK);
	outBufLength = dstLength + propsSize + COMPRESSED_SIZE;

	return outBuf;
}

byte* SevenZCompression::Uncompress(const byte *inBuf, size_t inBufLength, size_t &outBufLength)
{
	outBufLength = GetUncompressedSize(inBuf);
	size_t srcLen = inBufLength - LZMA_PROPS_SIZE - COMPRESSED_SIZE;

	byte *outBuf = new byte[outBufLength];

	SRes res = LzmaUncompress(
	    outBuf, (size_t *) &outBufLength,
	    &inBuf[LZMA_PROPS_SIZE + COMPRESSED_SIZE], &srcLen,
	    inBuf, LZMA_PROPS_SIZE);
	assert(res == SZ_OK);

	return outBuf;
}

size_t SevenZCompression::GetUncompressedSize(const byte *compressed) {
	size_t result = 0;

	memcpy(&result, compressed + LZMA_PROPS_SIZE, COMPRESSED_SIZE);

	return result;
}
