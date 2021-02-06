#include "Wrapper.h"

int main() {
	return 0;
}

CLIScumm::Wrapper::Wrapper(IConfigurationStore<System::Enum ^> ^ configureStore) {
	eventQueue = gcnew ConcurrentQueue<IGameEvent ^>();
	imageUpdated = gcnew CLIScumm::Wrapper::Wrapper::delCopyRectToScreen(this, &CLIScumm::Wrapper::Wrapper::UpdatePicturesToBeSentBuffer);
	pollEvent = gcnew CLIScumm::Wrapper::Wrapper::delPollEvent(this, &CLIScumm::Wrapper::Wrapper::pollEventWrapper);
	saveData = gcnew CLIScumm::Wrapper::Wrapper::delSaveData(this, &CLIScumm::Wrapper::Wrapper::SaveData);
	hasStarted = false;
	gameEventLock = gcnew Object();
	startLock = gcnew Object();

	SoundManagement::SoundOptions soundOptions = SoundManagement::SoundOptions();

	soundOptions.sampleRate = configureStore->GetValue<int>(SoundSettings::SampleRate);
	soundOptions.sampleSize = configureStore->GetValue<int>(SoundSettings::SampleSize);
	soundOptions.soundPollingFrequencyMs = configureStore->GetValue<int>(SoundSettings::SoundPollingFrequencyMs);
	soundOptions.serverFeedSize = configureStore->GetValue<int>(SoundSettings::ServerFeedSize);

	g_system = new NativeScummWrapper::NativeScummWrapperOSystem(soundOptions, static_cast<NativeScummWrapper::f_SendScreenBuffers>(Marshal::GetFunctionPointerForDelegate(imageUpdated).ToPointer()) //ToDo: Tidy these up as a whole they are a mess
	                                                             ,
	                                                             static_cast<NativeScummWrapper::f_PollEvent>(Marshal::GetFunctionPointerForDelegate(pollEvent).ToPointer()), static_cast<NativeScummWrapper::f_SaveFileData>(Marshal::GetFunctionPointerForDelegate(saveData).ToPointer()), static_cast<f_PlaySound>(Marshal::GetFunctionPointerForDelegate(GCHandle::Alloc(gcnew delPlaySound(this, &CLIScumm::Wrapper::Wrapper::PlaySound), GCHandleType::Normal).Target).ToPointer()));
	_gSystemCli = reinterpret_cast<NativeScummWrapper::NativeScummWrapperOSystem *>(g_system);
	_configureStore = configureStore;
	_soundIsRunning = false;
}

CopyRectToScreen ^ CLIScumm::Wrapper::OnCopyRectToScreen::get() {
	return copyRectToScreen;
}

void CLIScumm::Wrapper::OnCopyRectToScreen::set(CopyRectToScreen ^ copyRectToScreen) {
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

void CLIScumm::Wrapper::Init(AvailableGames game, Dictionary<System::String ^, cli::array<System::Byte> ^> ^ gameSaveData) {
	_gSystemCli->setGameSaveCache(Utilities::Converters::CreateSaveFileCacheFromDictionary(gameSaveData));

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
	ConfMan.setInt("autosave_period", 0, domain);
	ConfMan.setBool("originalsaveload", true);
}

void CLIScumm::Wrapper::RunGame(AvailableGames game, cli::array<System::Byte> ^ gameData, Dictionary<System::String ^, cli::array<Byte> ^> ^ gameSaveData, PlayAudio ^ playAudio) {
	if (!hasStarted) {
		Monitor::Enter(startLock);
		try {
			try {
				if (!hasStarted) {
					hasStarted = true;
					Monitor::Exit(startLock);

					_playAudio = playAudio;

					Init(game, gameSaveData);

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
	return Utilities::Converters::ManagedStringToCommonString(_configureStore->GetValue(game));
}

array<byte> ^ CLIScumm::Wrapper::MarshalByteBuffer(byte *buffer, int length) {
	cli::array<byte> ^ managedCompressedWholeScreenBuffer = gcnew cli::array<byte>(length);
	Marshal::Copy((System::IntPtr)buffer, managedCompressedWholeScreenBuffer, 0, length);
	return managedCompressedWholeScreenBuffer;
}

ScreenBuffer ^ CLIScumm::Wrapper::MarshalScreenBuffer(NativeScummWrapper::ScreenBuffer screenBuffer) {
	ScreenBuffer ^ result = gcnew ScreenBuffer();
	result->CompressedBuffer = MarshalByteBuffer(screenBuffer.buffer, screenBuffer.length);
	result->H = screenBuffer.h;
	result->W = screenBuffer.w;
	result->X = screenBuffer.x;
	result->Y = screenBuffer.y;
	result->CompressedPaletteBuffer = screenBuffer.compressedPalette != nullptr ? MarshalByteBuffer(screenBuffer.compressedPalette, screenBuffer.compressedPalletteLength) : nullptr;
	result->PaletteHash = screenBuffer.paletteHash;
	result->IgnoreColour = screenBuffer.ignoreColour;

	return result;
}

void CLIScumm::Wrapper::UpdatePicturesToBeSentBuffer(NativeScummWrapper::ScreenBuffer *unmanagedScreenBuffers, int length) {
	System::Collections::Generic::List<ScreenBuffer ^> ^ managedScreenScreenBuffers = gcnew System::Collections::Generic::List<ScreenBuffer ^>();

	for (int i = 0; i < length; i++) {
		ScreenBuffer ^ managedBuffer = MarshalScreenBuffer(unmanagedScreenBuffers[i]);

		managedScreenScreenBuffers->Add(managedBuffer);
	}

	if (OnCopyRectToScreen != nullptr) {
		OnCopyRectToScreen->Invoke(managedScreenScreenBuffers);
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

bool CLIScumm::Wrapper::SaveData(byte *data, int size, Common::String fileName) {
	array<System::Byte> ^ managedData = gcnew array<System::Byte>(size);
	Marshal::Copy((System::IntPtr)data, managedData, 0, size);
	return _saveData(managedData, gcnew System::String(fileName.c_str()));
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
