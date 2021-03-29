#include "pch.h"
#include "TestSoundOperation.h"
void TestSoundOperation::ProcessSound(SoundManagement::byte *soundData, int counter, int length, void *user) {
	SoundManagement::byte *newSoundData = new SoundManagement::byte[length];

	memcpy(newSoundData, soundData, length);
	for (int i = 0; i < length; i++) {
		newSoundData[i] += 1;
	}

	_soundConverted(newSoundData, counter, length, user);
}

void TestSoundOperation::Init(SoundManagement::SoundOptions soundOptions, SoundManagement::f_SoundOperated soundConverted) {
	_soundConverted = soundConverted;
}
