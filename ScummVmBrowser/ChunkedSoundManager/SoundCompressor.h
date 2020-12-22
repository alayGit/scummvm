#pragma once
#include "../ZLibCompression/ZLibCompression.h"
#include "SoundProcessor.h"

namespace SoundManagement {
class SoundCompressor : public SoundOperation {
	void ProcessSound(byte *soundData, int length, void *user) override;
	void Init(SoundOptions soundOptions, f_SoundOperated soundOperated) override;

private:
	f_SoundOperated _soundConverted;
};
} // namespace SoundManagement
