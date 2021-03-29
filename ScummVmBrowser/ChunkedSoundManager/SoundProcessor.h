#pragma once
#include "SoundOperation.h"
#include "DummySoundOperation.h"
#include <vector>
namespace SoundManagement {
typedef void(__stdcall *f_SoundOperated)(byte *, int, void *);
typedef void(__stdcall *f_PlaySound)(byte*, int, void* user);
class SoundProcessor {
public:
	SoundProcessor();
	virtual void Init(SoundOptions soundOptions, f_PlaySound soundOperationsCompleted);
	~SoundProcessor();
	void ProcessSound(byte* soundBytes, void *user);
	void OperateOnSound(byte *soundBytes, int length);
	void Flush();
	void AddOperation(SoundOperation* operation);
private:
	SoundOptions _soundOptions;
	f_PlaySound _soundOperationsCompleted;
	std::vector<SoundOperation*> _operations;
	int _operationCounter;
	void *_user;
	std::vector<byte> cachedSound;
	bool _isInited;
};
void __stdcall OperateOnSoundClb(byte *soundBytes, int size, void *user);
} // namespace SoundManagement
