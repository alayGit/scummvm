#include "Wrapper.h"

int main() {
	return 0;
}

CLIScumm::Wrapper::Wrapper(IConfigurationStore<System::Enum ^> ^ configureStore, ISaveCache ^ saveCache, ISaveDataEncoder^ byteEncoder, IScummTimer^ scummTimer) {
	eventQueue = gcnew ConcurrentQueue<IGameEvent ^>();
	imageUpdated = gcnew CLIScumm::Wrapper::Wrapper::delCopyRectToScreen(this, &CLIScumm::Wrapper::Wrapper::UpdatePicturesToBeSentBuffer);
	pollEvent = gcnew CLIScumm::Wrapper::Wrapper::delPollEvent(this, &CLIScumm::Wrapper::Wrapper::pollEventWrapper);
	saveData = gcnew CLIScumm::Wrapper::Wrapper::delSaveData(this, &CLIScumm::Wrapper::Wrapper::SaveData);
	hasStarted = false;
	gameEventLock = gcnew Object();
	startLock = gcnew Object();
	_saveCache = saveCache;
	_byteEncoder = byteEncoder;

	SoundManagement::SoundOptions soundOptions = SoundManagement::SoundOptions();

	soundOptions.sampleRate = configureStore->GetValue<int>(SoundSettings::SampleRate);
	soundOptions.sampleSize = configureStore->GetValue<int>(SoundSettings::SampleSize);
	soundOptions.soundPollingFrequencyMs = configureStore->GetValue<int>(SoundSettings::SoundPollingFrequencyMs);
	soundOptions.serverFeedSize = configureStore->GetValue<int>(SoundSettings::ServerFeedSize);
	_paletteManager = new NativeScummWrapperPaletteManager();
	_graphics = new NativeScummWrapperGraphics(static_cast<NativeScummWrapper::f_SendScreenBuffers>(Marshal::GetFunctionPointerForDelegate(imageUpdated).ToPointer()), _paletteManager);
	_saveFileManager = new SaveManager::UnmanagedSaveManagerWrapper(saveCache, static_cast<NativeScummWrapper::f_SaveFileData>(Marshal::GetFunctionPointerForDelegate(saveData).ToPointer()), byteEncoder, _paletteManager, _graphics);
	_unManagedScummTimerManagerWrapper = new UnmanagedScummTimerWrapper::UnmanagedScummTimerManagerWrapper(scummTimer);
	g_system = new NativeScummWrapper::NativeScummWrapperOSystem(soundOptions, //ToDo: Tidy these up as a whole they are a mess
	                                                             _graphics,
	                                                             static_cast<NativeScummWrapper::f_PollEvent>(Marshal::GetFunctionPointerForDelegate(pollEvent).ToPointer()), static_cast<NativeScummWrapper::f_SaveFileData>(Marshal::GetFunctionPointerForDelegate(saveData).ToPointer()), static_cast<f_PlaySound>(Marshal::GetFunctionPointerForDelegate(GCHandle::Alloc(gcnew delPlaySound(this, &CLIScumm::Wrapper::Wrapper::PlaySound), GCHandleType::Normal).Target).ToPointer()), _saveFileManager, _unManagedScummTimerManagerWrapper);
	_gSystemCli = reinterpret_cast<NativeScummWrapper::NativeScummWrapperOSystem *>(g_system);
	_configureStore = configureStore;
	_soundIsRunning = false;
}

CLIScumm::Wrapper::~Wrapper() {
	delete _paletteManager;
}

SendScreenBuffers ^ CLIScumm::Wrapper::SendScreenBuffers::get() {
	return copyRectToScreen;
}

void CLIScumm::Wrapper::SendScreenBuffers::set(ManagedCommon::Delegates::SendScreenBuffers ^ copyRectToScreen) {
	this->copyRectToScreen = copyRectToScreen;
}

SaveData ^ CLIScumm::Wrapper::OnSaveData::get() {
	return _saveData;
}

void CLIScumm::Wrapper::OnSaveData::set(ManagedCommon::Delegates::SaveData ^ saveData) {
	this->_saveData = saveData;
}

void CLIScumm::Wrapper::StartSound() {
	_gSystemCli->StartSound();
}

void CLIScumm::Wrapper::StopSound() {
	_gSystemCli->StopSound();
}
System::Drawing::Point CLIScumm::Wrapper::GetCurrentMousePosition() {
	NativeScummWrapper::MouseState mouseState = _gSystemCli->getGraphicsManager()->getMouseState();

	return System::Drawing::Point(mouseState.x, mouseState.y);
}

void CLIScumm::Wrapper::Init(AvailableGames game, System::String^ compressedAndEncodedGameSaveData, uint saveSlotToLoad) {

	_saveCache->SetCache(compressedAndEncodedGameSaveData);

	Common::String gamePath = GetGamePath(game);

	//TODO: Pass in a list of playable games
	g_system->getFilesystemFactory();
	Common::FSNode dir = Common::FSNode(gamePath);
	Common::FSList files;
	if (!dir.getChildren(files, Common::FSNode::kListAll)) {
		throw;
	}

	SearchMan.addDirectory(gamePath, dir);

	//PluginManager::loadAllPlugins()

	PluginManager::instance().init();           //TO DO FIX this hack, so that the plugin manager is not inited twice
	PluginManager::instance().loadAllPlugins(); // load plugins for cached plugin manager

	//Common::FSNode node = Common::FSNode(*_gamePath);
	//Common::FSList gameList = Common::FSList();
	//gameList.insert_at(0, node);

	DetectedGames detectionResults = EngineMan.detectGames(files).listDetectedGames();
	Common::String domain = EngineMan.createTargetForGame(detectionResults.front());
	//ConfMan.setActiveDomain(domain);
	//Common::String gfxMode = GUI::ThemeEngine::findModeConfigName(GUI::ThemeEngine::GraphicsMode::kGfxDisabled);
	//ConfMan.set("gui_renderer", gfxMode, domain);

	//Common::String domain = "kq3";
	ConfMan.addGameDomain(domain);
	ConfMan.setActiveDomain(domain);

	if (saveSlotToLoad != ManagedCommon::Constants::Constants::DoNotLoadSaveSlot && saveSlotToLoad <= _saveCache->SaveCount) {
		ConfMan.setInt("save_slot", saveSlotToLoad);
	}
	ConfMan.setInt("autosave_period", 0, domain);
	ConfMan.setBool("originalsaveload", true);
}

void CLIScumm::Wrapper::RunGame(AvailableGames game, cli::array<System::Byte> ^ gameData, System::String ^ gameSaveData, PlayAudio ^ playAudio, uint saveSlotToLoad) {
	if (!hasStarted) {
		Monitor::Enter(startLock);
		try {
			try {
				if (!hasStarted) {
					hasStarted = true;
					Monitor::Exit(startLock);

					_playAudio = playAudio;

					Init(game, gameSaveData, saveSlotToLoad);

					ConfMan.hasKey("debuglevel");
					scummvm_main(0, {});
					g_system->destroy();
				}
			} catch (std::exception e) {
				throw gcnew ManagedCommon::Exceptions::UnmanagedException(gcnew System::String(e.what()));
			} catch (std::exception *e) {
				throw gcnew ManagedCommon::Exceptions::UnmanagedException(gcnew System::String(e->what()));
			} catch (const char *e) {
				throw gcnew ManagedCommon::Exceptions::UnmanagedException(gcnew System::String(e));
			} catch (...) {
				throw gcnew ManagedCommon::Exceptions::UnmanagedException(gcnew System::String("Unknown"));
			}
		} finally {
			if (Monitor::IsEntered(startLock)) {
				Monitor::Exit(startLock);
			}
		}
	}
}

Common::String CLIScumm::Wrapper::GetGamePath(AvailableGames game) {
	return ScummToManagedMarshalling::Converters::ManagedStringToCommonString(_configureStore->GetValue(game));
}

array<byte> ^ CLIScumm::Wrapper::MarshalByteBuffer(byte *buffer, int length) {
	cli::array<byte> ^ managedCompressedWholeScreenBuffer = gcnew cli::array<byte>(length);
	Marshal::Copy((System::IntPtr)buffer, managedCompressedWholeScreenBuffer, 0, length);
	return managedCompressedWholeScreenBuffer;
}

ManagedCommon::Interfaces::ScreenBuffer ^ CLIScumm::Wrapper::MarshalScreenBuffer(NativeScummWrapper::ScreenBuffer screenBuffer) {
	ManagedCommon::Interfaces::ScreenBuffer ^ result = gcnew ManagedCommon::Interfaces::ScreenBuffer();
	result->PictureBuffer = MarshalByteBuffer(screenBuffer.buffer, screenBuffer.length);
	result->H = screenBuffer.h;
	result->W = screenBuffer.w;
	result->X = screenBuffer.x;
	result->Y = screenBuffer.y;
	result->PaletteBuffer = screenBuffer.paletteBuffer != nullptr ? MarshalByteBuffer(screenBuffer.paletteBuffer, screenBuffer.paletteBufferLength) : nullptr;
	result->PaletteHash = screenBuffer.paletteHash;
	result->IgnoreColour = screenBuffer.ignoreColour;

	return result;
}

void CLIScumm::Wrapper::UpdatePicturesToBeSentBuffer(NativeScummWrapper::ScreenBuffer *unmanagedScreenBuffers, int length) {
	System::Collections::Generic::List<ManagedCommon::Interfaces::ScreenBuffer ^> ^ managedScreenScreenBuffers = gcnew System::Collections::Generic::List<ManagedCommon::Interfaces::ScreenBuffer ^>();

	for (int i = 0; i < length; i++) {
		ManagedCommon::Interfaces::ScreenBuffer ^ managedBuffer = MarshalScreenBuffer(unmanagedScreenBuffers[i]);

		managedScreenScreenBuffers->Add(managedBuffer);
	}

	if (SendScreenBuffers != nullptr) {
		SendScreenBuffers->Invoke(managedScreenScreenBuffers);
	}
}

void CLIScumm::Wrapper::EnqueueGameEvent(IGameEvent ^ gameEvent) {
	Monitor::Enter(gameEventLock);
	gameEvent->GetType()->ToString()->Contains("Click");
	try {
		eventQueue->Enqueue(gameEvent);
	} finally {
		Monitor::Exit(gameEventLock);
	}
}

bool CLIScumm::Wrapper::SaveData(Common::String saveData) {
	return _saveData(Converters::CommonStringToManagedString(&saveData));
}

int gameCounter = 0;
bool CLIScumm::Wrapper::pollEventWrapper(Common::Event &event) {
	IGameEvent ^ gameEvent;
	bool result = false;

	try {
		Monitor::Enter(gameEventLock);
		if (eventQueue->TryPeek(gameEvent)) {
			if (gameEvent->HasEvents()) {
				event = *((Common::Event *)gameEvent->GetEvent().ToPointer());
				result = true;

				if (!gameEvent->HasEvents()) {
					eventQueue->TryDequeue(gameEvent);
				}
			}
		}
	} finally {
		Monitor::Exit(gameEventLock);
	}

	return result;
}

void CLIScumm::Wrapper::PlaySound(byte *buffer, int size, void *user) {
	byte *compressedSound = nullptr;
	try {
		cli::array<byte> ^ result = gcnew cli::array<byte>(size);
		Marshal::Copy((System::IntPtr)buffer, result, 0, size);

		_playAudio->Invoke(result);

	} finally {
		if (compressedSound != nullptr) {
			delete[] compressedSound;
		}

		if (buffer != nullptr) {
			delete[] buffer;
		}
	}
}

byte *CLIScumm::Wrapper::GetSoundSample(byte *buffer, int size) {
	return _gSystemCli->mixCallback(buffer, size);
}

void CLIScumm::Wrapper::Quit() {
	IGameEvent ^ gameEvent = gcnew CliScummEvents::SendQuit();

	EnqueueGameEvent(gameEvent);
}

void CLIScumm::Wrapper::ScheduleRedrawWholeScreen() {
	std::vector<NativeScummWrapper::ScreenBuffer> unmanagedWholeScreenBuffers;

	_gSystemCli->getGraphicsManager()->ScheduleRedrawWholeScreen();
}
