#include "7ZCompression.h"

void SevenZCompression::Compress(std::vector<unsigned char> &outBuf, const std::vector<unsigned char> &inBuf) {
	unsigned propsSize = LZMA_PROPS_SIZE;
	unsigned destLen = inBuf.size() + inBuf.size() / 3 + 128;
	outBuf.resize(propsSize + destLen);

	int res = LzmaCompress(
	    &outBuf[LZMA_PROPS_SIZE], &destLen,
	    &inBuf[0], inBuf.size(),
	    &outBuf[0], &propsSize,
	    -1, 0, -1, -1, -1, -1, -1);

	assert(propsSize == LZMA_PROPS_SIZE);
	assert(res == SZ_OK);

	outBuf.resize(propsSize + destLen);
}

void SevenZCompression::Uncompress(std::vector<unsigned char> &outBuf, const std::vector<unsigned char> &inBuf) {
	outBuf.resize(inBuf.size() * 1000);
	unsigned dstLen = outBuf.size();
	unsigned srcLen = inBuf.size() - LZMA_PROPS_SIZE;
	SRes res = LzmaUncompress(
	    &outBuf[0], &dstLen,
	    &inBuf[LZMA_PROPS_SIZE], &srcLen,
	    &inBuf[0], LZMA_PROPS_SIZE);
	outBuf.resize(dstLen); // If uncompressed data can be smaller
}
