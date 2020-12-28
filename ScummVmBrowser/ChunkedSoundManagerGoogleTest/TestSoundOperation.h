#pragma once
#include "../../ScummVmBrowser/ChunkedSoundManager/SoundOptions.h"
#include "../../ScummVmBrowser/ChunkedSoundManager/SoundOperation.h"
class TestSoundOperation : public SoundManagement::SoundOperation {
public:
	virtual void ProcessSound(SoundManagement::byte *soundData, int length, void *user) override;
	virtual void Init(SoundManagement::SoundOptions soundOptions, SoundManagement::f_SoundOperated soundConverted) override;

private:
	SoundManagement::f_SoundOperated _soundConverted;
};
