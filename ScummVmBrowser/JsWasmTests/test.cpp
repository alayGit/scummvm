#include "gtest/gtest.h"
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
	yEnc::Encoder encoder;

    uInt encodedSize;
	byte* encodedSoundBuffer = encoder.encode_buffer(sound, soundSize, encodedSize);

	std::string strEncoded = std::string(reinterpret_cast<char const *>(&encodedSoundBuffer[0]), encodedSize);

	delete[] encodedSoundBuffer;

	std::string strEncodedJsonArray = "[" + strEncoded + "]";

	size_t compressedLength;
	byte*compressed = SevenZCompression::Compress(reinterpret_cast<const byte*>(strEncodedJsonArray.c_str()), strEncodedJsonArray.size(), compressedLength, 3);

	uInt encodedJsonLength;
    byte* encodedJson = encoder.encode_buffer(compressed, compressedLength, encodedJsonLength);
	delete[] compressed;

	size_t uncompressedSize;
	byte *inflatedAndDecoded = JSWasm::InflateAndDecodeGameMessage(&encodedJson[0], encodedJsonLength, uncompressedSize);
	delete[] encodedJson;


	std::string strInflatedAndDecoded = std::string(reinterpret_cast<const char *>(strEncodedJsonArray.c_str()), uncompressedSize);

	ASSERT_EQ(strEncodedJsonArray, strInflatedAndDecoded);

	delete inflatedAndDecoded;
}
