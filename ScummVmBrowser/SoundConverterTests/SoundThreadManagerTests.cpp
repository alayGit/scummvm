#include "pch.h"
#include "CppUnitTest.h"
#include <thread>
#include "../ChunkedSoundManager/SoundThreadManager.h";

extern byte* GenerateRandomPcm(int length);
extern SoundManagement::SoundOptions GetSoundOptions(int sampleSize);

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
namespace SoundConverterTests
{

	TEST_CLASS(SoundThreadManagerTests)
	{
		const int SAMPLE_SIZE = 2000;
		TEST_METHOD_INITIALIZE(Initialize)
		{
			SoundManagement::SoundOptions _soundOptions = GetSoundOptions(SAMPLE_SIZE);

			_soundThreadManager.Init(
		       [this] (byte* buffer, int length) {
					Assert::AreEqual(SAMPLE_SIZE, length);
					
					return GenerateRandomPcm(SAMPLE_SIZE);
				},
				((SoundManagement::f_PlaySound) &SoundConvertedCallback),
			   _soundOptions,
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

				std::this_thread::sleep_for(std::chrono::milliseconds(50));

				Assert::IsTrue(_soundThreadManager.SoundIsRunning());

				_soundThreadManager.StopSound(false);

				int noTimesSoundConvertedCalledAfterStop = _noTimesSoundConvertedCalled;

				Assert::IsFalse(_soundThreadManager.SoundIsRunning());

				std::this_thread::sleep_for(std::chrono::milliseconds(50));

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

		void SoundConverted(std::vector<byte*> sounds)
		{
			Assert::IsTrue(sounds.size() > 0);
			_noTimesSoundConvertedCalled++;
		}

		static void __stdcall SoundConvertedCallback(std::vector<byte*> sounds, void* user)
		{
			((SoundConverterTests::SoundThreadManagerTests*) user)->SoundConverted(sounds);
		}

	private: 
		SoundManagement::SoundThreadManager _soundThreadManager;
		int _noTimesSoundConvertedCalled;
	};
}
