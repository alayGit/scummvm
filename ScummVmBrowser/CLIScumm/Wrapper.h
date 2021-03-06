#pragma once
#define FORBIDDEN_SYMBOL_ALLOW_ALL
#define _AFXDLL 
#define NO_COLORS 3
#define WINDOWS_IGNORE_PACKING_MISMATCH

#include <functional>
#include <objidl.h>
#include "../../engines/metaengine.h"
#include "../../common/config-manager.h"
#include "../../base/main.h"
#include "../../common/archive.h"
#include "../../common/fs.h"
#include "../../common/system.h"
#include "../nativeScummWrapper/nativeScummWrapperOSystem.h"
#include <stringapiset.h>
#include "../ChunkedSoundManager/SoundThreadManager.h"
#include "../ChunkedSoundManager/SoundOptions.h"
#include "../UnmanagedSaveManagerWrapper/UnmanagedSaveManagerWrapper.h"
#include "../nativeScummWrapper/PaletteManager.h"
#include "../UnmanagedScummTimerWrapper/UnmanagedScummTimerManagerWrapper.h"
#include <string.h>





using namespace System::Runtime::InteropServices;
//using namespace System::Drawing;
using namespace System::Drawing::Imaging;
using namespace System::Collections::Concurrent;
using namespace System::Collections::Generic;
using namespace System::Threading;
using namespace System::Threading::Tasks;
using namespace ManagedCommon::Interfaces;
using namespace ManagedCommon::Enums;
using namespace ManagedCommon::Delegates;
using namespace SoundManagement;
using namespace System::Collections::Generic;
using namespace UnmanagedScummTimerWrapper;
using namespace ManagedCommon::Interfaces;




ULONG_PTR m_gdiplusToken;

extern OSystem* g_system;
namespace CLIScumm {
	public ref class Wrapper :IWrapper {
	public:
		Wrapper(IConfigurationStore<System::Enum^>^ configureStore, ISaveCache^ saveCache, ISaveDataEncoder^ byteEncoder, IScummTimer^ scummtimer);
	    ~Wrapper();
		virtual void EnqueueGameEvent(IGameEvent^ keyboardEvent);
		virtual void Quit();
	    virtual void CLIScumm::Wrapper::RunGame(AvailableGames game, cli::array<Byte> ^ gameData, System::String ^ compressedAndEncodedGameSaveData, PlayAudio ^ playSound, uint saveSlotToLoad);
		virtual void CLIScumm::Wrapper::ScheduleRedrawWholeScreen();
		virtual property SendScreenBuffers^ SendScreenBuffers {
		    ManagedCommon::Delegates::SendScreenBuffers ^ get();
		    void set(ManagedCommon::Delegates::SendScreenBuffers ^ copyRectToScreen);
		}
		virtual property SaveData^ OnSaveData {
			SaveData^ get();
			void set(SaveData^ saveData);
		}
		virtual void StartSound();
		virtual void StopSound();
	    virtual System::Drawing::Point GetCurrentMousePosition();
	private:
		void Init(AvailableGames game, System::String^ compressedAndEncodedGameSaveData, uint saveSlotToLoad);
		bool SaveData(Common::String data);
		bool pollEventWrapper(Common::Event& event);
		void PlaySound(byte* buffer, int size, void* user);
		byte* GetSoundSample(byte* buffer, int size);
		Common::String GetGamePath(AvailableGames game);
	    void CLIScumm::Wrapper::UpdatePicturesToBeSentBuffer(NativeScummWrapper::ScreenBuffer *unmanagedScreenBuffers, int length);
		ConcurrentQueue<IGameEvent^>^ eventQueue;
		delegate void delCopyRectToScreen(NativeScummWrapper::ScreenBuffer*, int length);
	    array<byte>^ MarshalByteBuffer(byte *buffer, int length);
	    ManagedCommon::Interfaces::ScreenBuffer ^ MarshalScreenBuffer(NativeScummWrapper::ScreenBuffer screenBuffer);
		delegate bool delPollEvent(Common::Event& event);
		delegate bool delSaveData(Common::String saveData);
		delegate void delPlaySound(byte* buffer, int size, void* user);
		delegate byte* delGetSound(byte* buffer, int size);
		delCopyRectToScreen^ imageUpdated;
		delPollEvent^ pollEvent;
		delSaveData^ saveData;
		Object^ gameEventLock;
		Object^ startLock;
		bool hasStarted;
	    ManagedCommon::Delegates::SendScreenBuffers^ copyRectToScreen;
		ManagedCommon::Delegates::SaveData^ _saveData;
		bool _redrawWholeScreenOnNextFrame;
		IConfigurationStore<System::Enum^>^ _configureStore;
		NativeScummWrapper::NativeScummWrapperOSystem* _gSystemCli;
		PlayAudio^ _playAudio;
		bool _soundIsRunning;
		Common::SaveFileManager* _saveFileManager;
	    ISaveCache^ _saveCache;
	    ISaveDataEncoder ^ _byteEncoder;
	    NativeScummWrapperPaletteManager *_paletteManager;
	    NativeScummWrapperGraphics *_graphics;
	    UnmanagedScummTimerWrapper::UnmanagedScummTimerManagerWrapper* _unManagedScummTimerManagerWrapper;
	};

}
