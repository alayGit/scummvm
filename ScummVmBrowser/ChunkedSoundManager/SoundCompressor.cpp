#include "SoundCompressor.h"

void SoundManagement::SoundCompressor::Init(SoundOptions soundOptions, f_SoundOperated soundOperated) {
	if (_isInited) {
		throw std::exception("Cannot init twice, soundCompressor");
	}

	_isInited = true;
	_soundConverted = soundOperated;
}

SoundManagement::SoundCompressor::SoundCompressor() {
	_isInited = false;
	_soundConverted = nullptr;
}

void SoundManagement::SoundCompressor::ProcessSound(byte *soundData, int length, void *user) {

	if (!_isInited)
	{
		throw std::exception("Cannot process sound on sound compressor until inited");
	}

	ZLibCompression::ZLibCompression compressor;

	int compressedLength;
	byte *compressedSound = compressor.Compress(soundData, length, compressedLength);
	_soundConverted(compressedSound, compressedLength, user);
}


