#include "pch.h"
#include "CppUnitTest.h"
#include <thread>
#include "../ChunkedSoundManager/SoundThreadManager.h";
#include "../chunkedSoundManager/DummySoundOperation.h"

extern byte* GenerateRandomPcm(int length);
extern SoundManagement::SoundOptions GetSoundOptions(int sampleSize);

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
namespace SoundConverterTests
{

	TEST_CLASS(SoundThreadManagerTests)
	{
		const int SAMPLE_SIZE = 2000;
	    SoundManagement::SoundProcessor* _soundProcessor;

		TEST_METHOD_CLEANUP(Cleanup) {
		    delete _soundProcessor;
	    }

		TEST_METHOD_INITIALIZE(Initialize)
		{
			SoundManagement::SoundOptions _soundOptions = GetSoundOptions(SAMPLE_SIZE);

			_soundProcessor = new SoundManagement::SoundProcessor();
		    _soundProcessor->AddOperation(new SoundManagement::DummySoundOperation());
		    _soundProcessor->Init(_soundOptions, ((SoundManagement::f_PlaySound)&SoundConvertedCallback));
			_soundThreadManager.Init(
		       [this] (byte* buffer, int length) {
					Assert::AreEqual(SAMPLE_SIZE, length);
					
					return GenerateRandomPcm(SAMPLE_SIZE);
				},
			   _soundOptions,
			   _soundProcessor,
			   this
			);

			_noTimesSoundConvertedCalled = 0;
		}

		TEST_METHOD(DoesSoundStartAndStop)
		{
			Assert::IsFalse(_soundThreadManager.SoundIsRunning());

			for (int i = 0, noTimesCalledLasttime= 0; i < 10; i++)
			{
				_soundThreadManager.StartSound();

				std::this_thread::sleep_for(std::chrono::milliseconds(500));

				Assert::IsTrue(_soundThreadManager.SoundIsRunning());

				_soundThreadManager.StopSound(false);

				int noTimesSoundConvertedCalledAfterStop = _noTimesSoundConvertedCalled;

				Assert::IsFalse(_soundThreadManager.SoundIsRunning());

				std::this_thread::sleep_for(std::chrono::milliseconds(500));

				Assert::AreEqual(_noTimesSoundConvertedCalled, noTimesSoundConvertedCalledAfterStop);
				
				if (i != 0)
				{
					_noTimesSoundConvertedCalled > noTimesCalledLasttime;
				}
				else
				{
					Assert::IsTrue(_noTimesSoundConvertedCalled > 0);
				}

				noTimesCalledLasttime = _noTimesSoundConvertedCalled;
			}
		    int y = 4;
		}

		TEST_METHOD(DoesSoundNotStartAfterPermanantStop)
		{
			Assert::IsFalse(_soundThreadManager.SoundIsRunning());

			_soundThreadManager.StartSound();

			std::this_thread::sleep_for(std::chrono::milliseconds(50));

			Assert::IsTrue(_soundThreadManager.SoundIsRunning());

			_soundThreadManager.StopSound(true);

			_soundThreadManager.StartSound();

			int noTimesSoundConvertedCalledBeforeStop = _noTimesSoundConvertedCalled;

			Assert::IsFalse(_soundThreadManager.SoundIsRunning());

			std::this_thread::sleep_for(std::chrono::milliseconds(50));

			Assert::AreEqual(_noTimesSoundConvertedCalled, noTimesSoundConvertedCalledBeforeStop);
		}


		TEST_METHOD(ThrowsIfStartSoundWithoutInit) {
		    bool exceptionThrown = false;

			SoundManagement::SoundThreadManager soundThreadManager;
		    try {
			    soundThreadManager.StartSound();
		    } catch (std::exception e) {
			    exceptionThrown = true;
		    }

		    Assert::IsTrue(exceptionThrown);
	    }

		TEST_METHOD(ThrowsIfStopSoundWithoutInit) {
		    bool exceptionThrown = false;

		    SoundManagement::SoundThreadManager soundThreadManager;
		    try {
			    soundThreadManager.StopSound(false);
		    } catch (std::exception e) {
			    exceptionThrown = true;
		    }

		    Assert::IsTrue(exceptionThrown);
	    }

		TEST_METHOD(ThrowsIfInitTwice) {
		    bool exceptionThrown = false;

		    SoundManagement::SoundThreadManager soundThreadManager;
		    try {
			    _soundThreadManager.Init(nullptr, SoundManagement::SoundOptions(), nullptr, nullptr);
		    } catch (std::exception e) {
			    exceptionThrown = true;
		    }

		    Assert::IsTrue(exceptionThrown);
	    }

		void SoundConverted(byte* sounds, int size)
		{
			Assert::IsTrue(size > 0);
			_noTimesSoundConvertedCalled++;
		}

		static void __stdcall SoundConvertedCallback(byte* sounds, int size, void* user)
		{
			((SoundConverterTests::SoundThreadManagerTests*) user)->SoundConverted(sounds, size);
		}

	private: 
		SoundManagement::SoundThreadManager _soundThreadManager;
		int _noTimesSoundConvertedCalled;
	};
}
