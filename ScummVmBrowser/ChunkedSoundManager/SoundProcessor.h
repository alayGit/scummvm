#pragma once
#include "SoundOperation.h"
#include "SoundConversion.h";
#include "SoundCompressor.h";
#include "DummySoundOperation.h"
#include "C:\scumm\ScummVmBrowser\LaunchDebugger\LaunchDebugger.h"
#include <vector>
namespace SoundManagement {
const int NO_OPERATIONS = 2;
typedef void(__stdcall *f_SoundOperated)(byte *, int, void *);
typedef void(__stdcall *f_PlaySound)(byte*, int, void* user);
class SoundProcessor {
public:
	SoundProcessor();
	void Init(SoundOptions soundOptions, f_PlaySound soundOperationsCompleted);
	~SoundProcessor();
	void ProcessSound(byte* soundBytes, void *user);
	void OperateOnSound(byte *soundBytes, int length);
	void Flush();
private:
	SoundOptions _soundOptions;
	f_PlaySound _soundOperationsCompleted;
	SoundOperation** operations;
	int _operationCounter;
	void *_user;
	std::vector<byte> cachedSound;
};
void __stdcall OperateOnSoundClb(byte *soundBytes, int size, void *user);
} // namespace SoundManagement
