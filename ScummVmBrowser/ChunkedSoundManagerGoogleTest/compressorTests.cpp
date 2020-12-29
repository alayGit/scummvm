#include "pch.h"
#include "gtest/gtest.h"
#include "../ChunkedSoundManager/SoundCompressor.h"
#include "../ZLibCompression/ZLibCompression.h"

const int LENGTH_DATA_TO_COMPRESS = 3;
const int DATA_BYTE = 5;

byte *dataToCompress = new byte[LENGTH_DATA_TO_COMPRESS];
bool callBackCalled;

void __stdcall CompressorCallback(SoundManagement::byte *data, int length, void *user) {
	EXPECT_TRUE(length > 0);

	ZLibCompression::ZLibCompression compressor;

	int outputLength;
	byte* result = compressor.Decompress(data, length, outputLength);

	EXPECT_EQ(LENGTH_DATA_TO_COMPRESS, outputLength);

	for (int i = 0; i < LENGTH_DATA_TO_COMPRESS; i++) {
		EXPECT_EQ(DATA_BYTE, result[i]);
	}

	EXPECT_EQ(dataToCompress, user);

	delete[] dataToCompress;
	delete[] result;

	callBackCalled = true;
}

TEST(SoundCompressorCompressesBytes) {
	callBackCalled = false;

	memset(dataToCompress, DATA_BYTE, LENGTH_DATA_TO_COMPRESS);

	SoundManagement::SoundCompressor soundCompressor;
	soundCompressor.Init(SoundManagement::SoundOptions(), &CompressorCallback);
	soundCompressor.ProcessSound(dataToCompress, LENGTH_DATA_TO_COMPRESS, dataToCompress);

	EXPECT_TRUE(callBackCalled);
}

TEST(CannotInitTwice) {
	SoundManagement::SoundCompressor soundCompressor;
	soundCompressor.Init(SoundManagement::SoundOptions(), &CompressorCallback);
	EXPECT_THROW(soundCompressor.Init(SoundManagement::SoundOptions(), &CompressorCallback), std::exception);
}

TEST(CannotProcessSoundWithoutIniting) {
	const int DATA_LENGTH = 4;
	byte *testData = new byte[DATA_LENGTH];
	SoundManagement::SoundCompressor soundCompressor;

	EXPECT_THROW(soundCompressor.ProcessSound(testData, DATA_LENGTH, &testData), std::exception);

	delete[] testData;
}
