#include "DummySoundOperation.h"

void SoundManagement::DummySoundOperation::ProcessSound(byte *soundData, int counter, int length, void *user) {

	byte *processedSoundData = new byte[length];
	memcpy(processedSoundData, soundData, length);
	_soundConverted(processedSoundData, counter, length, user);
}

void SoundManagement::DummySoundOperation::Init(SoundOptions soundOptions, f_SoundOperated soundOperated) {
	_soundConverted = soundOperated;
}
