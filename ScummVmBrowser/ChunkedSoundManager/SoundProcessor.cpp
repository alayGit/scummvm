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
	operations[0] = new DummySoundOperation();
	operations[1] = new DummySoundOperation();

	_operationCounter = 0;

	for (int i = 0; i < NO_OPERATIONS; i++) {
		operations[i]->Init(soundOptions, OperateOnSoundClb);
	}
}

void SoundManagement::SoundProcessor::ProcessSound(byte *pcm, void *user) {
	_user = user;
	OperateOnSound(pcm, _soundOptions.sampleSize);
}

void SoundManagement::SoundProcessor::OperateOnSound(byte *soundBytes, int length) {
	if (_operationCounter < NO_OPERATIONS) {
		operations[_operationCounter++]->ProcessSound(soundBytes, length, this);
		delete[] soundBytes;
	}
	else
	{
		std::vector<byte*> result;
		result.push_back(soundBytes);

		_soundOperationsCompleted(result, _user);
	}
}

void __stdcall SoundManagement::OperateOnSoundClb(byte *soundBytes, int length, void *user) {
    ((SoundProcessor *)user)->OperateOnSound(soundBytes, length);
}
