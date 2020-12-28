#include "pch.h"
#include "gtest/gtest.h"
#include "TestSoundOperation.h";
#include "../../ScummVmBrowser/ChunkedSoundManager/SoundProcessor.h"
const int NO_OPERATIONS = 5;

SoundManagement::byte *resultData;
int resultLength;
void *resultUser;

void __stdcall Callback(SoundManagement::byte *data, int length, void *user) {
	resultData = data;
	resultLength = length;
	resultUser = user;
}

class SoundProcessorFixture : public ::testing::Test {
	public:
	SoundManagement::SoundProcessor _soundProcessor;
	SoundManagement::SoundOptions _soundOptions;

	SoundProcessorFixture() {
		resultData = 0;
		resultLength = 0;
		resultUser = 0;


		for (int i = 0; i < NO_OPERATIONS; i++) {
			_soundOptions.serverFeedSize = 3;
			_soundOptions.sampleSize = 1;
			_soundProcessor.AddOperation(new TestSoundOperation());
		}

		_soundProcessor.Init(_soundOptions, &Callback);
	}
};

TEST_F(SoundProcessorFixture, DoesSoundProcessorCallAllOperations) {
	int expectedUserValue = 0;

	for (int i = 0; i < _soundOptions.serverFeedSize; i++) {
		SoundManagement::byte *data = new SoundManagement::byte[1];
		data[0] = 0;

		_soundProcessor.ProcessSound(data, &expectedUserValue);
	}

	for (int i = 0; i < _soundOptions.serverFeedSize; i++) {
		EXPECT_TRUE(resultData[i] == NO_OPERATIONS);
	}

	EXPECT_EQ(_soundOptions.serverFeedSize, resultLength);
	EXPECT_EQ(&expectedUserValue, resultUser);
}

TEST_F(SoundProcessorFixture, DoesSoundProcessorFlushSendCachedSound) {
	int expectedUserValue = 0;

	for (int i = 0; i < _soundOptions.serverFeedSize - 1; i++) {
		SoundManagement::byte *data = new SoundManagement::byte[1];
		data[0] = 0;

		_soundProcessor.ProcessSound(data, &expectedUserValue);
	}

	_soundProcessor.Flush();

	for (int i = 0; i < _soundOptions.serverFeedSize - 1; i++) {
		EXPECT_TRUE(resultData[i] == NO_OPERATIONS);
	}

	EXPECT_EQ(_soundOptions.serverFeedSize - 1, resultLength);
	EXPECT_EQ(&expectedUserValue, resultUser);
}

TEST_F(SoundProcessorFixture, DataDoesNotGetSentIfThereIsLessDataThanServerFeed) {
	int expectedUserValue = 0;

	for (int i = 0; i < _soundOptions.serverFeedSize - 1; i++) {
		SoundManagement::byte *data = new SoundManagement::byte[1];
		data[0] = 0;

		_soundProcessor.ProcessSound(data, &expectedUserValue);
	}

	EXPECT_EQ(0, resultData);
	EXPECT_EQ(0, resultLength);
	EXPECT_EQ(0, resultUser);
}

TEST_F(SoundProcessorFixture, CannotInitTwice) {
	EXPECT_THROW(_soundProcessor.Init(SoundManagement::SoundOptions(), &Callback), std::exception);
}

TEST_F(SoundProcessorFixture, CannotAddAfterInit) {
	TestSoundOperation _testOp;

	EXPECT_THROW(_soundProcessor.AddOperation(&_testOp), std::exception);
}

TEST_F(SoundProcessorFixture,CannotInitWithNoOperations) {
	EXPECT_THROW(SoundManagement::SoundProcessor().Init(SoundManagement::SoundOptions(), &Callback), std::exception);
}
