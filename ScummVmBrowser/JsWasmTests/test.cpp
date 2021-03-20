#include "pch.h"
#include <vector>
#include "../JSWasm/ProcessGameMessages.h"
#include "../yEncoder/yEncoder.h"
#include "../7ZCompression/7ZCompression.h"
#include "../JSWasm/ProcessGameMessages.h";
TEST(ProcessGameMessages, CanInflateAndDecodeGameMessage) {
	Crc32 crc = Crc32();
	crc.bytes = 0;
	crc.crc = 0;

	const int soundSize = 10;
	byte sound[soundSize] = {12, 112, 33, 44, 55, 66, 55, 66, 122, 33};
	std::vector<byte> encodedSoundBuffer;
	yEnc::Encoder encoder;

	uInt col = 0;

	encoder.encode_buffer(sound, encodedSoundBuffer, soundSize, &crc, &col);

	std::string strEncoded = std::string(reinterpret_cast<char const *>(&encodedSoundBuffer[0]), encodedSoundBuffer.size());

	std::string strEncodedJsonArray = "[" + strEncoded + "]";

	size_t compressedLength;
	byte *compressed = SevenZCompression::Compress(reinterpret_cast<const byte*>(strEncodedJsonArray.c_str()), strEncodedJsonArray.size(), compressedLength);
	std::vector<byte> encodedJson;

	encoder.encode_buffer(compressed, encodedJson, compressedLength, &crc, &col);

	delete[] compressed;

	std::string inflatedAndDecoded = JSWasm::InflateAndDecodeGameMessage(&encodedJson[0], encodedJson.size());

	ASSERT_EQ(strEncodedJsonArray, inflatedAndDecoded);
}
