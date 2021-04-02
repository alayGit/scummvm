#pragma once
#include "SoundConversion.h";
#include "SoundOptions.h"
#ifdef NATIVE_CODE
#include <future>
#endif
#include "SoundCompressor.h";
#include "SoundConversion.h";
#include "SoundProcessor.h"
#include <cassert>
#include <functional>
#include <mutex>
#include <string>
#include <windows.h>
namespace SoundManagement {
typedef std::function<byte *(byte *, int)> f_GetSoundSample;
class SoundThreadManager {
public:
	SoundThreadManager();
	~SoundThreadManager();
	void Init(f_GetSoundSample getSoundSample, SoundOptions soundOptions, SoundProcessor *soundProcessor, void *user = nullptr);
	void StartSound();
	void StopSound(bool soundIsStoppedForeverPriorToDestructor);
	bool SoundIsRunning();

private:
	SoundProcessor *_soundProcessor;
	f_GetSoundSample _getSoundSample;
	bool _soundIsRunning;
	bool _soundIsStoppedForeverPriorToDestructor;

	#ifdef NATIVE_CODE
	std::future<void> _soundThread;
	#endif

	HANDLE _stopSoundMutex;
	SoundOptions _soundOptions;
	void *_user;
};
} // namespace SoundManagement
