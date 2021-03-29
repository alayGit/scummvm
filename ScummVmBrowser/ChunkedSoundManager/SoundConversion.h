#pragma once
#include <vector>
#include "../../ExternalLibraries/include/bass.h"
#include "../../ExternalLibraries/include/bassenc.h"
#include "../../ExternalLibraries/include/bassenc_flac.h"
#include "../../ExternalLibraries/include/bassmix.h"
#include <mmreg.h>
#include "SoundOptions.h"
#include "SoundProcessor.h"


namespace SoundManagement
{
	class SoundConverter: public SoundOperation
	{
	public:
	    SoundConverter();
		virtual void Init(SoundOptions soundOptions, f_SoundOperated soundConverted);
		~SoundConverter();
		void ConvertPcmToFlac(byte* pcm, int length, void* user);
		void GetEncodedData(HENCODE handle, DWORD channel, const void* buffer, DWORD length);
	    void ProcessSound(byte *pcm, int counter, int length, void *user) override;
	private:
		SoundOptions _soundOptions;
		std::vector<byte> _workingBuffer;
		SoundManagement::f_SoundOperated _soundConverted; //TODO:Fix we are force to do this because we cannot use member functions in C callbacks.
		void* _user;
	    bool _isInited;
	    int _counter;
	};

}
