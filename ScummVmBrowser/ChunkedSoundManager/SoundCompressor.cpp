#include "SoundCompressor.h"

void SoundManagement::SoundCompressor::Init(SoundOptions soundOptions, f_SoundOperated soundOperated) {
	_soundConverted = soundOperated;
}

void SoundManagement::SoundCompressor::ProcessSound(byte *soundData, int length, void *user) {
	ZLibCompression::ZLibCompression compressor;

	int compressedLength;
	byte *compressedSound = compressor.Compress(soundData, length, compressedLength);
	_soundConverted(compressedSound, compressedLength, user);
}


