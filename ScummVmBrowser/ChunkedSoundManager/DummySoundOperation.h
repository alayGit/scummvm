#pragma once
#include "SoundOperation.h";
namespace SoundManagement {
class DummySoundOperation : public SoundOperation {
	void ProcessSound(byte *soundData, int counter, int length, void *user) override;
	void Init(SoundOptions soundOptions, f_SoundOperated soundOperated) override;

private:
	f_SoundOperated _soundConverted;
};
} // namespace SoundManagement
