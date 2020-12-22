#pragma once

#define FORBIDDEN_SYMBOL_ALLOW_ALL

#include "nativeScummWrapperOSystem.h"

NativeScummWrapper::NativeScummWrapperOSystem::NativeScummWrapperOSystem(SoundManagement::SoundOptions soundOptions, f_SendScreenBuffers sendScreenBuffers, f_PollEvent queueEvent, f_SaveFileData saveData, SoundManagement::f_PlaySound playSound) : ModularBackend() {
	_mixerImpl = nullptr;	
	_soundOptions = soundOptions;
	_fsFactory = new WindowsFilesystemFactory();
	_cliGraphicsManager = new NativeScummWrapperGraphics(sendScreenBuffers);
	_eventSource = new NativeScummWrapperEvents(queueEvent, [this](int x, int y) {
		_graphicsManager->warpMouse(x, y);
	});
	ModularBackend::_graphicsManager = _cliGraphicsManager;
	_mutexManager = new StandardMutexManager();
	_savefileManager = new NativeScummVmSaveManager(saveData);
	_soundThreadManager = new SoundManagement::SoundThreadManager();
	_playSound = playSound;
}




NativeScummWrapper::NativeScummWrapperOSystem::~NativeScummWrapperOSystem() {
	_soundThreadManager->StopSound(true);
	delete _soundThreadManager;
	ModularBackend::~ModularBackend();
}

void NativeScummWrapper::NativeScummWrapperOSystem::initBackend() {
	Audio::MixerImpl* mixer = new Audio::MixerImpl(_soundOptions.sampleRate);
	mixer->setReady(true);
	_mixer = mixer;     //Super class type variable required by ScummVM
	_mixerImpl = mixer; //The subclass variable for our convenience

	std::function<byte* (byte*, int)> mixCallBack = [&](byte* x, int y) {
		return this->mixCallback(x, y);
	};

	_soundThreadManager->Init(mixCallBack, _playSound, _soundOptions);
	_timerManager = new DefaultTimerManager();
	ModularBackend::initBackend();
}

uint32 NativeScummWrapper::NativeScummWrapperOSystem::getMillis(bool skipRecord) {
	return (uint32)clock();
}

void NativeScummWrapper::NativeScummWrapperOSystem::delayMillis(uint msecs) {
	//TODO Implement
}

void NativeScummWrapper::NativeScummWrapperOSystem::getTimeAndDate(TimeDate& t) const {
	//TODO Implement
}

void NativeScummWrapper::NativeScummWrapperOSystem::logMessage(LogMessageType::Type type, const char* message) {
}

void NativeScummWrapper::NativeScummWrapperOSystem::setGameSaveCache(SaveFileCache* cache) {
	((NativeScummVmSaveManager*)_savefileManager)->setGameSaveCache(cache);
}

const int NO_CHANNELS = 2; //TODO: Read this value from Scumm VM don't assume 2
byte* NativeScummWrapper::NativeScummWrapperOSystem::mixCallback(byte* samples, int sampleSize) {
	_mixerImpl->mixCallback((uint8_t*)samples, sampleSize);

	return samples;
}

void NativeScummWrapper::NativeScummWrapperOSystem::StartSound() {
	//if (!_mixerImpl->isReady())
	//{
	//	throw "Attempted to start the sound without the mixer being ready";
	//}

	_soundThreadManager->StartSound();
}

void NativeScummWrapper::NativeScummWrapperOSystem::quit() {

}

void NativeScummWrapper::NativeScummWrapperOSystem::StopSound() {
	_soundThreadManager->StopSound(false);
}

NativeScummWrapper::NativeScummWrapperGraphics* NativeScummWrapper::NativeScummWrapperOSystem::getGraphicsManager() {
	return _cliGraphicsManager;
}

Common::EventSource* NativeScummWrapper::NativeScummWrapperOSystem::getDefaultEventSource() {
	return _eventSource;
}
