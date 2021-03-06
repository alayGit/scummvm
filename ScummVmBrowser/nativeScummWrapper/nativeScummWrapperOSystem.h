#pragma once
#include "scummVm.h"
#include <time.h>
#include "nativeScummWrapperGraphics.h"
#include "nativeScummVMWrapperEvents.h"
#include "nativeScummVmWrapperSaveMemStream.h"
#include "standardMutexManager.h"
#include "../chunkedSoundManager/SoundOptions.h"
#include "../chunkedSoundManager/SoundThreadManager.h"
#include "../chunkedSoundManager/SoundProcessor.h"
#include "../chunkedSoundManager/SoundCompressor.h"
#include "../chunkedSoundManager/SoundConversion.h"
#include "../common/timer.h"
#include "C:\scumm\ScummVmBrowser\LaunchDebugger\LaunchDebugger.h"
#include <assert.h>

namespace NativeScummWrapper {
	class NativeScummWrapperOSystem: public ModularBackend {
	public:
	    NativeScummWrapperOSystem(SoundManagement::SoundOptions soundOptions, NativeScummWrapperGraphics* cliGraphicsManager, NativeScummWrapper::f_PollEvent queueEvent, NativeScummWrapper::f_SaveFileData saveData, SoundManagement::f_PlaySound soundConverted, Common::SaveFileManager *_saveFileManager, Common::TimerManager* unmanagedScummTimerWrapper);
	    ~NativeScummWrapperOSystem();
		virtual void initBackend() override;
		virtual uint32 getMillis(bool skipRecord = false) override;
		virtual void delayMillis(uint msecs) override;
		virtual void getTimeAndDate(TimeDate &t) const override;
		virtual void logMessage(LogMessageType::Type type, const char *message) override;
	    byte* mixCallback(byte *samples, int sampleSize);
		void StartSound();
		void StopSound();
	    NativeScummWrapper::NativeScummWrapperGraphics *getGraphicsManager();
	    virtual void quit() override;
	    virtual Common::SeekableReadStream *createConfigReadStream() override;
	protected:
		NativeScummWrapperEvents *_eventSource;

		virtual Common::EventSource *getDefaultEventSource();

	private:
		SoundManagement::SoundOptions _soundOptions;
		SoundManagement::f_PlaySound _playSound;
		Audio::MixerImpl *_mixerImpl;
		SoundManagement::SoundThreadManager *_soundThreadManager;
		NativeScummWrapper::NativeScummWrapperGraphics*_cliGraphicsManager;
	    SoundManagement::SoundProcessor* _soundProcessor;
	    Common::TimerManager* _unmanagedScummTimerWrapper;
	};
} // namespace NativeScummWrapper
