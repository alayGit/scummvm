#pragma once
#include "../ChunkedSoundManager/SoundOptions.h"
#include <iostream>

SoundManagement::SoundOptions GetSoundOptions(int sampleSize)
{
	SoundManagement::SoundOptions _soundOptions;

	_soundOptions.sampleRate = 16000;
	_soundOptions.sampleSize = sampleSize;
	_soundOptions.soundPollingFrequencyMs = 20;

	return _soundOptions;
}

byte* GenerateRandomPcm(int length)
{
	byte* result = new byte[length];

	for (int i = 0; i < length; i++)
	{
		result[i] = rand() % MAXBYTE; //Gives a range of positive and negative values. RAND_MAX isn't big enough, so we times the result by 2
		std::cout << std::to_string(result[i]) + "\n";
	}

	return result;
}