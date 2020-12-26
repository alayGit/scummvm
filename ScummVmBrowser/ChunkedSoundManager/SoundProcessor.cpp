#include "SoundProcessor.h"

SoundManagement::SoundProcessor::SoundProcessor() {
	_operationCounter = 0;
	_soundOperationsCompleted = nullptr;
	_soundOptions = SoundOptions();
	_user = nullptr;
	operations = nullptr;
}

SoundManagement::SoundProcessor::~SoundProcessor() {
	for (int i = 0; i < NO_OPERATIONS; i++) {
		delete operations[i];
	}
	delete operations;
}

void SoundManagement::SoundProcessor::Init(SoundOptions soundOptions, f_PlaySound soundOperationsCompleted) {
	_soundOptions = soundOptions;
	_soundOperationsCompleted = soundOperationsCompleted;

	operations = new SoundOperation *[NO_OPERATIONS];
	operations[0] = new SoundConverter();
	operations[1] = new SoundCompressor();

	_operationCounter = 0;

	for (int i = 0; i < NO_OPERATIONS; i++) {
		operations[i]->Init(soundOptions, OperateOnSoundClb);
	}
}

void SoundManagement::SoundProcessor::ProcessSound(byte *pcm, void *user) {
	_user = user;
	for (int i = 0; i < _soundOptions.sampleSize; i++) {
		cachedSound.push_back(pcm[i]);
	}

	if (cachedSound.size() / _soundOptions.sampleSize == _soundOptions.serverFeedSize) {
		Flush();
	}

	delete[] pcm;
}

void SoundManagement::SoundProcessor::OperateOnSound(byte *soundBytes, int length) {
	if (_operationCounter < NO_OPERATIONS) {
		operations[_operationCounter++]->ProcessSound(soundBytes, length, this);
		delete[] soundBytes;
	}
	else
	{
		_soundOperationsCompleted(soundBytes, length, _user);
	}
}

void SoundManagement::SoundProcessor::Flush() {
	if (cachedSound.size()) {
		byte *joinedPcm = new byte[cachedSound.size()];
		memcpy(joinedPcm, &cachedSound[0], cachedSound.size());
		OperateOnSound(joinedPcm, cachedSound.size());
		cachedSound.clear();
		_operationCounter = 0;
	}
}

void __stdcall SoundManagement::OperateOnSoundClb(byte *soundBytes, int length, void *user) {
    ((SoundProcessor *)user)->OperateOnSound(soundBytes, length);
}
