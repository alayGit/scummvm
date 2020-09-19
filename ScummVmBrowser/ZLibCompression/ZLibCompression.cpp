#include "pch.h"

#include "ZLibCompression.h"

byte *ZLibCompression::ZLibCompression::Compress(byte *inputBuffer, int inputLength, int &outputLength) {
	byte *outputBuffer = nullptr;

	// STEP 1.
	// deflate a into b. (that is, compress a into b)

	// zlib struct
	z_stream defstream;
	defstream.zalloc = Z_NULL;
	defstream.zfree = Z_NULL;
	defstream.opaque = Z_NULL;
	// setup "a" as the input and "b" as the compressed output
	defstream.avail_in = (uInt)inputLength;
	defstream.next_in = (Bytef *)inputBuffer; // input char array

	// the actual compression work.
	deflateInit(&defstream, Z_BEST_COMPRESSION);

	int deflateBounds = deflateBound(&defstream, inputLength);
	outputBuffer = new Byte[deflateBounds];
	defstream.next_out = (Bytef *)outputBuffer; // output char array
	defstream.avail_out = deflateBounds;

	deflate(&defstream, Z_FINISH);
	deflateEnd(&defstream);

	outputLength = defstream.total_out;

	return outputBuffer;
}

byte *ZLibCompression::ZLibCompression::Decompress(byte *inputBuffer, int inputLength, int &outputLength) {
	Byte *outputBuffer = nullptr;

	int outputBufferBounds = compressBound(inputLength);
	outputBuffer = new Byte[outputBufferBounds];

	z_stream infstream;
	infstream.zalloc = Z_NULL;
	infstream.zfree = Z_NULL;
	infstream.opaque = Z_NULL;
	// setup "b" as the input and "c" as the compressed output
	infstream.avail_in = (uInt)inputLength + 1;     // size of input
	infstream.next_in = (Bytef *)inputBuffer;       // input char array
	infstream.avail_out = (uInt)inputLength * 1000; // size of output
	infstream.next_out = (Bytef *)outputBuffer;     // output char array
	// the actual DE-compression work.

	inflateInit(&infstream);
	inflate(&infstream, Z_NO_FLUSH);
	inflateEnd(&infstream);

	outputLength = infstream.total_out;

	return outputBuffer;
}
