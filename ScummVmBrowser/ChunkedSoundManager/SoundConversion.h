#pragma once
#include <vector>
#include "../../ExternalLibraries/include/bass.h"
#include "../../ExternalLibraries/include/bassenc.h"
#include "../../ExternalLibraries/include/bassenc_aac.h"
#include "../../ExternalLibraries/include/bassmix.h"
#include <mmreg.h>
#include "SoundOptions.h"


namespace SoundManagement
{
	typedef unsigned char byte;
	typedef void(__stdcall* f_SoundConverted) (byte*, int, void*);

	class SoundConverter
	{
	public:
		SoundConverter(SoundOptions soundOptions, f_SoundConverted soundConverted);
		~SoundConverter();
		void ConvertPcmToAac(byte* pcm, int noChannels, void* user);
		void GetEncodedData(HENCODE handle, DWORD channel, const void* buffer, DWORD length);
	private:
		SoundOptions _soundOptions;
		std::vector<byte> _workingBuffer;
		SoundManagement::f_SoundConverted _soundConverted; //TODO:Fix we are force to do this because we cannot use member functions in C callbacks.
		void* _user;
	};

}
