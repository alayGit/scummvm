#include "SoundProcessor.h"

SoundManagement::SoundProcessor::SoundProcessor() {
	_operationCounter = 0;
	_soundOperationsCompleted = nullptr;
	_soundOptions = SoundOptions();
	_user = nullptr;
	_isInited = false;
}

SoundManagement::SoundProcessor::~SoundProcessor() {
	for (int i = 0; i < _operations.size(); i++) {
		delete _operations[i];
	}
}

void SoundManagement::SoundProcessor::Init(SoundOptions soundOptions, f_PlaySound soundOperationsCompleted) {
	if (_isInited) {
		throw std::exception("Cannot init twice, soundProcessor");
	}

	if (_operations.size() == 0)
	{
		throw std::exception("No operations added, and init called, add operations first and then call init");
	}

	_soundOptions = soundOptions;
	_soundOperationsCompleted = soundOperationsCompleted;

	_operationCounter = 0;

	for (int i = 0; i < _operations.size(); i++) {
		_operations[i]->Init(soundOptions, OperateOnSoundClb);
	}

	_isInited = true;
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
	if (_operationCounter < _operations.size()) {
		_operations[_operationCounter++]->ProcessSound(soundBytes, length, this);
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

void SoundManagement::SoundProcessor::AddOperation(SoundOperation* operation) {
	if (_isInited) {
		throw std::exception("Cannot add once inited, add first, and then init");
	}

	_operations.push_back(operation);
}

void __stdcall SoundManagement::OperateOnSoundClb(byte *soundBytes, int length, void *user) {
    ((SoundProcessor *)user)->OperateOnSound(soundBytes, length);
}
