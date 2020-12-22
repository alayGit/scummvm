#pragma once
#include "SoundOperation.h"
#include "SoundConversion.h";
#include "SoundCompressor.h";
#include <vector>
namespace SoundManagement {
const int NO_OPERATIONS = 2;
typedef void(__stdcall *f_SoundOperated)(byte *, int, void *);
typedef void(__stdcall *f_PlaySound)(std::vector<byte*>, void* user);
class SoundProcessor {
public:
	SoundProcessor();
	void Init(SoundOptions soundOptions, f_PlaySound soundOperationsCompleted);
	~SoundProcessor();
	void ProcessSound(byte* soundBytes, void *user);
	void OperateOnSound(byte *soundBytes, int length);
private:
	SoundOptions _soundOptions;
	f_PlaySound _soundOperationsCompleted;
	SoundOperation** operations;
	int _operationCounter;
	void *_user;
};
void __stdcall OperateOnSoundClb(byte *soundBytes, int length, void *user);
} // namespace SoundManagement
