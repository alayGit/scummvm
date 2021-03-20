#include "ProcessGameMessages.h"

std::string JSWasm::InflateAndDecodeGameMessage(byte *deflatedAndEncoded, int deflatedAndEncodedLength) {
	Crc32 crc = Crc32();
	crc.bytes = 0;
	crc.crc = 0;

	yEnc::Encoder encoder;
	std::vector<byte> decodedBuffer;
	bool escape = false;

	encoder.decode_buffer(deflatedAndEncoded, decodedBuffer, deflatedAndEncodedLength, &crc, &escape);

	size_t unCompressedLength;
	byte *uncompressed = SevenZCompression::Uncompress(&decodedBuffer[0], decodedBuffer.size(),unCompressedLength);

	std::string result = std::string(reinterpret_cast<char const *>(uncompressed), unCompressedLength);

	delete[] uncompressed;

	return result;
}
