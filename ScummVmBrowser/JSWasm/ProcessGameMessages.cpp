#include "ProcessGameMessages.h"

byte* JSWasm::InflateAndDecodeGameMessage(byte *deflatedAndEncoded, int deflatedAndEncodedLength, size_t& uncompressedLength) {
	yEnc::Encoder encoder;
	bool escape = false;

	uInt decodedBufferLength;
	Byte* decodedBuffer = encoder.decode_buffer(deflatedAndEncoded, deflatedAndEncodedLength, decodedBufferLength);

	return SevenZCompression::Uncompress(decodedBuffer, decodedBufferLength, uncompressedLength);
}
