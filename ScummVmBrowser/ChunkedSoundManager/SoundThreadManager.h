#pragma once
#include "SoundConversion.h";
#include "SoundOptions.h"
#include <thread>
#include <cassert>
#include <functional>
#include <windows.h>
#include <string>
#include <mutex>
namespace SoundManagement
{
	typedef std::function<byte* (byte*, int)> f_GetSoundSample;
	class SoundThreadManager
	{
	public:
		SoundThreadManager();
		~SoundThreadManager();
		void Init(f_GetSoundSample getSoundSample, f_SoundConverted playSound, SoundOptions soundOptions, void* user = nullptr);
		void StartSound();
		void StopSound(bool soundIsStoppedForeverPriorToDestructor);
		bool SoundIsRunning();
	private:
		SoundConverter* _soundConverter;
		f_GetSoundSample _getSoundSample;
		bool _soundIsRunning;
		bool _soundIsStoppedForeverPriorToDestructor;
		std::thread* _soundThread;
		HANDLE _stopSoundMutex;
		SoundOptions _soundOptions;
		void* _user;
	};
}
