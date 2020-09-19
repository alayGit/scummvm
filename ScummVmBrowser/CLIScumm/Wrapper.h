#pragma once
#define FORBIDDEN_SYMBOL_ALLOW_ALL
#define DISPLAY_DEFAULT_WIDTH   320
#define DISPLAY_DEFAULT_HEIGHT  200
#define DISPLAY_DEFAULT_SIZE  6400
#define NO_BYTES_PER_PIXEL 4
#define _AFXDLL 
#define NO_COLORS 3
#define WINDOWS_IGNORE_PACKING_MISMATCH

#include <functional>
#include <objidl.h>
#include "../../engines/metaengine.h"
#include "Converters.h";
#include "../../common/config-manager.h"
#include "../../base/main.h"
#include "../../common/archive.h"
#include "../../common/fs.h"
#include "../../common/system.h"
#include "../nativeScummWrapper/nativeScummWrapperOSystem.h"
#include <stringapiset.h>
#include "../ChunkedSoundManager/SoundThreadManager.h"
#include "../ChunkedSoundManager/SoundOptions.h"
#include <string.h>
#include "../ZLibCompression/ZLibCompression.h"





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




ULONG_PTR m_gdiplusToken;

extern OSystem* g_system;
namespace CLIScumm {



	public ref class Wrapper :IWrapper {
	public:
		Wrapper(IConfigurationStore<System::Enum^>^ configureStore);
		~Wrapper();
		virtual void EnqueueGameEvent(IGameEvent^ keyboardEvent);
		virtual void Quit();
		virtual void CLIScumm::Wrapper::RunGame(AvailableGames game, cli::array<Byte>^ gameData, Dictionary<System::String^, cli::array<Byte>^>^ gameSaveData, PlayAudio^ playSound);
		virtual array<Byte>^ CLIScumm::Wrapper::GetWholeScreen(int% width, int% height);
		virtual property CopyRectToScreen^ OnCopyRectToScreen {
			CopyRectToScreen^ get();
			void set(CopyRectToScreen^ copyRectToScreen);
		}
		virtual property SaveData^ OnSaveData {
			SaveData^ get();
			void set(SaveData^ saveData);
		}
		virtual void StartSound();
		virtual void StopSound();
		virtual System::Drawing::Point GetCurrentMousePosition();
	private:
		void Init(AvailableGames game, Dictionary<System::String^, cli::array<Byte>^>^);
		void CLIScumm::Wrapper::ScreenUpdated(const void* buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor* color, byte ignore, bool isMouseUpdate, int noUpdates);
		void Blot(int x, int y, int w, int h, int noUpdates);
		bool SaveData(byte* data, int size, Common::String fileName);
		void InitScreen();
		bool pollEventWrapper(Common::Event& event);
		void PlaySound(byte* buffer, int size, void* user);
		byte* GetSoundSample(byte* buffer, int size);
		Common::String GetGamePath(AvailableGames game);
		System::Collections::Generic::List<ScreenBuffer^>^ GetListOfScreenBufferFromSinglePictureArray(cli::array<Byte>^ pictureArray, int x, int y, int w, int h);
		void CLIScumm::Wrapper::UpdatePicturesToBeSentBuffer(cli::array<Byte>^ pictureArray, int noUpdates, int x, int y, int w, int h, ManagedCommon::Enums::Actions::DrawingAction drawingAction);
		ConcurrentQueue<IGameEvent^>^ eventQueue;
		delegate void delCopyRectToScreen(const void* buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor* color, byte ignore, bool isMouseUpdate, int noUpdates);
	    array<byte>^ MarshalBuffer(byte *buffer, int length);
		delegate bool delPollEvent(Common::Event& event);
		delegate bool delSaveData(byte* saveData, int, Common::String fileName);
		delegate void delPlaySound(byte* buffer, int size, void* user);
		delegate byte* delGetSound(byte* buffer, int size);
		delegate void  delBlot(int, int, int, int, int);
		delCopyRectToScreen^ imageUpdated;
		delPollEvent^ pollEvent;
		delSaveData^ saveData;
		Object^ gameEventLock;
		Object^ startLock;
		Object^ _wholeScreenBufferLock;
		bool hasStarted;
		CopyRectToScreen^ copyRectToScreen;
		ManagedCommon::Delegates::SaveData^ _saveData;
		bool _redrawWholeScreenOnNextFrame;
		byte* _wholeScreenBuffer;
		byte* _wholeScreenBufferNoMouse;
	    int _wholeScreenBufferLength;
		IConfigurationStore<System::Enum^>^ _configureStore;
	    void CLIScumm::Wrapper::UpdatePictureBuffer(byte *pictureArray, const void *buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor *color, byte ignore);
	    void CLIScumm::Wrapper::UpdateWholeScreenBuffer(byte *pictureArray, byte *wholeScreenBuffer, int x, int y, int w, int h);
		NativeScummWrapper::NativeScummWrapperOSystem* _gSystemCli;
		PlayAudio^ _playAudio;
		bool _soundIsRunning;
		System::Collections::Generic::List<ScreenBuffer^>^ _picturesToBeSentBuffer;
	};

}
