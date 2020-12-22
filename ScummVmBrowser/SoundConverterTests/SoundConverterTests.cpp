#include "pch.h"
#include "CppUnitTest.h"
#include "../ChunkedSoundManager/SoundConversion.h"
#include <stdlib.h>
#include <iostream>
#include<string>
#include<thread>
#include "Common.h"
#include "bassflac.h"
#include "bass.h"
using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace SoundManagement;


namespace SoundConverterTests
{

	TEST_CLASS(SoundConverterTests)
	{
	public:
		const int NO_VALUES = 2000; //Must be even

		SoundConverterTests()
		{
		    _converter.Init(GetSoundOptions(NO_VALUES), (SoundManagement::f_SoundOperated)&encodeCallback);
			_wasAfterDecodeCallbackMade = false;
			_wasAfterEncodeCallbackMade = false;
			_wasAfterDecodeCallbackCompleted = false;

			_afterDecodeComparisonCounter = 0;
			_expectedPcm = 0;
		}

		TEST_METHOD(DoesConvertToFlac)
		{
			_expectedPcm = GenerateRandomPcm(NO_VALUES);
			_converter.ConvertPcmToFlac(_expectedPcm, this);

			std::this_thread::sleep_for(std::chrono::milliseconds(200));
			Assert::IsTrue(_wasAfterEncodeCallbackMade);
			Assert::IsTrue(_wasAfterDecodeCallbackMade);
			Assert::IsTrue(_wasAfterDecodeCallbackCompleted);

			delete[] _expectedPcm;
		}

		void afterEncode(byte* buffer, int length)
		{
			_wasAfterEncodeCallbackMade = true;

			DoesFlacConvertToOriginalPcm(buffer, length);
		}

		void afterDecode(HENCODE handle, byte* buffer, int length)
		{
			_wasAfterDecodeCallbackMade = true;

			DWORD encoderStatus = BASS_Encode_IsActive(handle);

			for (int i = 0; i < length; i++, _afterDecodeComparisonCounter++)
			{
				Assert::AreEqual(buffer[i], _expectedPcm[_afterDecodeComparisonCounter]);
		    }

			if (_afterDecodeComparisonCounter == NO_VALUES) //TODO: The encoder status never changes, so I have to do this check instead of checking the encoder status
			{
				Assert::AreEqual(NO_VALUES, _afterDecodeComparisonCounter);
				_wasAfterDecodeCallbackCompleted = true;
			}
		}

		static void __stdcall encodeCallback(byte* buffer, int length, void* user)
		{
			((SoundConverterTests*)user)->afterEncode(buffer, length);
		}
		
		static void CALLBACK decodeCallback(HENCODE handle, DWORD channel, void* buffer, DWORD length, void* user)
		{
			((SoundConverterTests*)user)->afterDecode(handle, (byte*)buffer, length);
		}

	private:
		bool _wasAfterEncodeCallbackMade;
		bool _wasAfterDecodeCallbackMade;
		bool _wasAfterDecodeCallbackCompleted;
		byte* _expectedPcm;
		SoundManagement::SoundConverter _converter;
		int _afterDecodeComparisonCounter;

		void DoesFlacConvertToOriginalPcm(byte* flac, int length)
		{
			const int BufferSize = 256;
			HSTREAM stream = BASS_FLAC_StreamCreateFile(true, flac, 0, length, BASS_STREAM_DECODE);

		    BASS_Encode_Start(stream,(char*) nullptr, BASS_ENCODE_PCM | BASS_ENCODE_NOHEAD,(ENCODEPROC*) &decodeCallback, this);
			int result = 1;
			while (result != -1) { // processing loop...
				byte* buffer = new byte[BufferSize];

				result = BASS_ChannelGetData(stream, buffer, BufferSize); // process the decoder until it fails/ends

				delete[] buffer;
			}

			BASS_StreamFree(stream); // free the decoder (and WAV writer due to AUTOFREE)
		}
	};
}

