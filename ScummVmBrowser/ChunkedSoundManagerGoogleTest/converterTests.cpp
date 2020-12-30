#include "pch.h"
#include "gtest/gtest.h"
#include "../ChunkedSoundManager/SoundConversion.h"

const int FLAC_PREAMBLE_LENGTH = 4;
const SoundManagement::byte FLAC_PREAMBLE[FLAC_PREAMBLE_LENGTH] = {102, 76, 97, 67};

bool converterCallbackCalled = false;

void __stdcall SoundConverterCallback(SoundManagement::byte *data, int length, void *user) {
	converterCallbackCalled = true;

	EXPECT_TRUE(length > 0);
	EXPECT_EQ(&converterCallbackCalled, user);

	for (int i = 0; i < FLAC_PREAMBLE_LENGTH; i++)
	{
		EXPECT_EQ(FLAC_PREAMBLE[i], data[i]);
	}
}

TEST(SoundConverterTests, DoesSoundConvert) {
	const int SOUND_LENGTH = 1000;
	SoundManagement::SoundConverter soundConverter;
	SoundManagement::SoundOptions options;
	options.sampleRate = 14000;
	options.sampleSize = SOUND_LENGTH;

	soundConverter.Init(options, &SoundConverterCallback);
	
	byte* testSound = new byte[SOUND_LENGTH];

	memset(testSound, 2, SOUND_LENGTH);

	soundConverter.ConvertPcmToFlac(testSound, SOUND_LENGTH, &converterCallbackCalled);

	delete[] testSound;

	EXPECT_TRUE(converterCallbackCalled);
}

TEST(SoundConverterTests, CannotInitTwice) {
	SoundManagement::SoundConverter soundConverter;
	soundConverter.Init(SoundManagement::SoundOptions(), &SoundConverterCallback);

	EXPECT_THROW(soundConverter.Init(SoundManagement::SoundOptions(), &SoundConverterCallback), std::exception);
}

TEST(SoundConverterTests, CannotConvertWithoutIniting) {
	const int SOUND_LENGTH = 34;
	SoundManagement::SoundConverter soundConverter;
	byte *testData = new byte[SOUND_LENGTH];

	EXPECT_THROW(soundConverter.ConvertPcmToFlac(testData, SOUND_LENGTH, nullptr), std::exception);
}


