#include "ProcessGameMessages.h"

byte* JSWasm::InflateAndDecodeGameMessage(byte *deflatedAndEncoded, int deflatedAndEncodedLength, size_t& uncompressedLength) {
	Crc32 crc = Crc32();
	crc.bytes = 0;
	crc.crc = 0;

	yEnc::Encoder encoder;
	std::vector<byte> decodedBuffer;
	bool escape = false;

	encoder.decode_buffer(deflatedAndEncoded, decodedBuffer, deflatedAndEncodedLength, &crc, &escape);

	return SevenZCompression::Uncompress(&decodedBuffer[0], decodedBuffer.size(), uncompressedLength);
}
