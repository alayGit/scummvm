#include "SoundThreadManager.h";


SoundManagement::SoundThreadManager::SoundThreadManager()
{
	_stopSoundMutex = CreateSemaphore( //Cannot use 'std::mutex due to an iteration issue with CLR
		NULL,           // default security attributes
		1,  // initial count
		1,  // maximum count
		NULL);          // unnamed semaphore
	_soundIsRunning = false;
	_soundIsStoppedForeverPriorToDestructor = false;
	_soundOptions = SoundOptions();
	_soundThread = nullptr;
	_user = nullptr;
}

SoundManagement::SoundThreadManager::~SoundThreadManager()
{
	CloseHandle(_stopSoundMutex);
}

void SoundManagement::SoundThreadManager::Init(f_GetSoundSample getSoundSample, f_PlaySound playSound, SoundOptions soundOptions, void *user) {
	_soundOptions = soundOptions;
	_getSoundSample = getSoundSample;
	_user = user;
	_soundThread = nullptr;
	_soundProcessor.Init(soundOptions, playSound);
}

void SoundManagement::SoundThreadManager::StartSound()
{
	if (!_soundIsRunning && !_soundIsStoppedForeverPriorToDestructor)
	{
		WaitForSingleObject(_stopSoundMutex, INFINITE);

		if (!_soundIsRunning && !_soundIsStoppedForeverPriorToDestructor)
		{
			_soundIsRunning = true;

			_soundThread = new std::thread( //TODO: Memory 
				[this] {
					while (_soundIsRunning)
					{
						try
						{
				
							if (_soundIsRunning && _getSoundSample != nullptr) //TODO: We really want to throw an exception in 'StartSound' is 'getSoundSample' is null but we can't as SignalR does not guarantee the order of the Init and StartSound calls. When we have an RPC framework in place we will
							{
								WaitForSingleObject(_stopSoundMutex, INFINITE);
								if (_soundIsRunning && _getSoundSample != nullptr)
								{
									byte* samples = new byte[_soundOptions.sampleSize];

									samples = _getSoundSample(samples, _soundOptions.sampleSize);

									_soundProcessor.ProcessSound(samples, _user);
								}
								ReleaseSemaphore(_stopSoundMutex, 1, NULL);
							}

							std::this_thread::sleep_for(std::chrono::milliseconds(_soundOptions.soundPollingFrequencyMs));
						}
						catch (std::string e)
						{
							//TODO: Log
							int x = 4;
						}
					}
				}
			);

			ReleaseSemaphore(_stopSoundMutex, 1, NULL);
		}
	}
}

void SoundManagement::SoundThreadManager::StopSound(bool stopSoundForeverPriorToDestructor)
{
	if ((_soundIsRunning || stopSoundForeverPriorToDestructor) && !_soundIsStoppedForeverPriorToDestructor)
	{
		WaitForSingleObject(_stopSoundMutex, INFINITE);

		if (!_soundIsStoppedForeverPriorToDestructor)
		{
			_soundIsRunning = false;
			_soundIsStoppedForeverPriorToDestructor = stopSoundForeverPriorToDestructor;
		}
		ReleaseSemaphore(_stopSoundMutex, 1, NULL);

		if (_soundThread != nullptr)
		{
			_soundThread->join();
		}
	}
}

bool SoundManagement::SoundThreadManager::SoundIsRunning()
{
	WaitForSingleObject(_stopSoundMutex, INFINITE);
	
	bool result = _soundIsRunning;

	ReleaseSemaphore(_stopSoundMutex, 1, NULL);

	return result;
}
