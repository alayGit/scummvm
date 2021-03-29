#pragma once
#include "SoundOperation.h"
#include "DummySoundOperation.h"
#include <vector>
#include <thread>
namespace SoundManagement {
typedef void(__stdcall *f_SoundOperated)(byte *, int, int, void *);
typedef void(__stdcall *f_PlaySound)(byte*, int, void* user);
class SoundProcessor {
public:
	SoundProcessor();
	virtual void Init(SoundOptions soundOptions, f_PlaySound soundOperationsCompleted);
	~SoundProcessor();
	void ProcessSound(byte* soundBytes, void *user);
	void Flush();
	void AddOperation(SoundOperation* operation);
	void OperateOnSound(byte *soundBytes, int counter, int length);
private:
	SoundOptions _soundOptions;
	f_PlaySound _soundOperationsCompleted;
	std::vector<SoundOperation*> _operations;
	void *_user;
	std::vector<byte> cachedSound;
	bool _isInited;
};
void __stdcall OperateOnSoundClb(byte *soundBytes, int size, int counter, void *user);
} // namespace SoundManagement
