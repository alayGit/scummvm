#pragma once
#include "SoundOptions.h"

#include <functional>
namespace SoundManagement {
typedef unsigned char byte;
typedef void(__stdcall *f_SoundOperated)(byte *, int, int, void *);
const int NO_CHANNELS = 2;

class SoundOperation {

public:
	virtual void ProcessSound(byte *soundData, int counter, int length, void *user) = 0;
	virtual void Init(SoundOptions soundOptions, f_SoundOperated soundConverted) = 0;

protected:
	SoundManagement::SoundOptions options;
};
} // namespace SoundManagement
