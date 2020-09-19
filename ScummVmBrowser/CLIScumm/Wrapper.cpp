#include "Wrapper.h"

int main() {
	return 0;
}

CLIScumm::Wrapper::Wrapper(IConfigurationStore<System::Enum^>^ configureStore)
{
	eventQueue = gcnew ConcurrentQueue<IGameEvent^>();
	imageUpdated = gcnew CLIScumm::Wrapper::Wrapper::delCopyRectToScreen(this, &CLIScumm::Wrapper::Wrapper::ScreenUpdated);
	pollEvent = gcnew CLIScumm::Wrapper::Wrapper::delPollEvent(this, &CLIScumm::Wrapper::Wrapper::pollEventWrapper);
	saveData = gcnew CLIScumm::Wrapper::Wrapper::delSaveData(this, &CLIScumm::Wrapper::Wrapper::SaveData);
	hasStarted = false;
	gameEventLock = gcnew Object();
	startLock = gcnew Object();
	_wholeScreenBufferLock = gcnew Object();
	_picturesToBeSentBuffer = gcnew System::Collections::Generic::List<ScreenBuffer^>();

	SoundManagement::SoundOptions soundOptions = SoundManagement::SoundOptions();

	soundOptions.sampleRate = configureStore->GetValue<int>(SoundSettings::SampleRate);
	soundOptions.sampleSize = configureStore->GetValue<int>(SoundSettings::SampleSize);
	soundOptions.soundPollingFrequencyMs = configureStore->GetValue<int>(SoundSettings::SoundPollingFrequencyMs);

	g_system = new NativeScummWrapper::NativeScummWrapperOSystem(soundOptions, static_cast<NativeScummWrapper::f_CopyRect>(Marshal::GetFunctionPointerForDelegate(imageUpdated).ToPointer()) //ToDo: Tidy these up as a whole they are a mess
		, static_cast<NativeScummWrapper::f_PollEvent>(Marshal::GetFunctionPointerForDelegate(pollEvent).ToPointer())
		, static_cast<NativeScummWrapper::f_SaveFileData>(Marshal::GetFunctionPointerForDelegate(saveData).ToPointer())
		, static_cast<f_SoundConverted>(Marshal::GetFunctionPointerForDelegate(GCHandle::Alloc(gcnew delPlaySound(this, &CLIScumm::Wrapper::Wrapper::PlaySound), GCHandleType::Normal).Target).ToPointer())
		, static_cast<NativeScummWrapper::f_Blot>(Marshal::GetFunctionPointerForDelegate(GCHandle::Alloc(gcnew delBlot(this, &CLIScumm::Wrapper::Wrapper::Blot), GCHandleType::Normal).Target).ToPointer())
	);
	_gSystemCli = reinterpret_cast<NativeScummWrapper::NativeScummWrapperOSystem*>(g_system);
	_wholeScreenBuffer = new byte[DISPLAY_DEFAULT_WIDTH * DISPLAY_DEFAULT_HEIGHT * NO_BYTES_PER_PIXEL];
	_wholeScreenBufferLength = DISPLAY_DEFAULT_WIDTH * DISPLAY_DEFAULT_HEIGHT * NO_BYTES_PER_PIXEL;
	_wholeScreenBufferNoMouse = new byte[_wholeScreenBufferLength];
	_configureStore = configureStore;
	_soundIsRunning = false;
}

CLIScumm::Wrapper::~Wrapper()
{
	delete[] _wholeScreenBuffer;
	delete[] _wholeScreenBufferNoMouse;
}

CopyRectToScreen^ CLIScumm::Wrapper::OnCopyRectToScreen::get() {
	return copyRectToScreen;
}

void CLIScumm::Wrapper::OnCopyRectToScreen::set(CopyRectToScreen^ copyRectToScreen) {
	this->copyRectToScreen = copyRectToScreen;
}

SaveData^ CLIScumm::Wrapper::OnSaveData::get() {
	return _saveData;
}

void CLIScumm::Wrapper::OnSaveData::set(ManagedCommon::Delegates::SaveData^ saveData) {
	this->_saveData = saveData;
}

void CLIScumm::Wrapper::StartSound()
{
	_gSystemCli->StartSound();
}

void CLIScumm::Wrapper::StopSound()
{
	_gSystemCli->StopSound();
}

System::Drawing::Point CLIScumm::Wrapper::GetCurrentMousePosition()
{
	NativeScummWrapper::MouseState mouseState = _gSystemCli->getGraphicsManager()->getMouseState();

	return System::Drawing::Point(mouseState.x, mouseState.y);
}

void CLIScumm::Wrapper::Init(AvailableGames game, Dictionary<System::String^, cli::array<System::Byte>^>^ gameSaveData)
{
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

	PluginManager::instance().init(); //TO DO FIX this hack, so that the plugin manager is not inited twice
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

void CLIScumm::Wrapper::RunGame(AvailableGames game, cli::array<Byte>^ gameData, Dictionary<System::String^, cli::array<Byte>^>^ gameSaveData, PlayAudio^ playAudio)
{
	if (!hasStarted)
	{
		Monitor::Enter(startLock);
		try
		{

			try
			{
				if (!hasStarted)
				{
					hasStarted = true;
					Monitor::Exit(startLock);

					/*auto fn1 = &std::bind(&OSystem_Cli::mixCallback, *_gSystemCli, std::placeholders::_1, std::placeholders::_2);*/
				 // auto fn2 = &std::bind(fn1, *_gSystemCli, std::placeholders::_2, std::placeholders::_3);

					/*std::function<byte* (byte*, int)> f = [&](byte* x, int y) {
						return _gSystemCli->mixCallback(x, y);
					};*/

					_playAudio = playAudio;

					Init(game, gameSaveData);

					InitScreen();
					ConfMan.hasKey("debuglevel");
					scummvm_main(0, {});
					g_system->destroy();
				}
			}
			catch (std::exception e)
			{
				throw gcnew ManagedCommon::Exceptions::UnmanagedException(gcnew System::String(e.what()));
			}
			catch (std::exception* e)
			{
				throw gcnew ManagedCommon::Exceptions::UnmanagedException(gcnew System::String(e->what()));
			}
			catch (const char* e)
			{
				throw gcnew ManagedCommon::Exceptions::UnmanagedException(gcnew System::String(e));
			}
			catch (...)
			{
				throw gcnew ManagedCommon::Exceptions::UnmanagedException(gcnew System::String("Unknown"));
			}
		}
		finally
		{
			if (Monitor::IsEntered(startLock))
			{
				Monitor::Exit(startLock);
			}
		}
	}
}

Common::String CLIScumm::Wrapper::GetGamePath(AvailableGames game)
{
	return Utilities::Converters::ManagedStringToCommonString(_configureStore->GetValue(game));
}

System::Collections::Generic::List<ScreenBuffer^>^ CLIScumm::Wrapper::GetListOfScreenBufferFromSinglePictureArray(cli::array<Byte>^ pictureArray, int x, int y, int w, int h)
{
	System::Collections::Generic::List<ScreenBuffer^>^ updates = gcnew System::Collections::Generic::List<ScreenBuffer^>();

	ScreenBuffer^ screenBuffer = gcnew ScreenBuffer();

	screenBuffer->Buffer = pictureArray;
	screenBuffer->X = x;
	screenBuffer->Y = y;
	screenBuffer->W = w;
	screenBuffer->H = h;

	updates->Add(screenBuffer);

	return updates;
}

array<byte> ^ CLIScumm::Wrapper::MarshalWholeScreenBuffer(byte *wholeScreenBuffer) {
	array<byte>^ result = gcnew array<byte>(_wholeScreenBufferLength);
	Marshal::Copy((System::IntPtr) wholeScreenBuffer, result, 0, _wholeScreenBufferLength);

	return result;
}

void CLIScumm::Wrapper::UpdatePicturesToBeSentBuffer(cli::array<Byte>^ pictureArray, int noUpdates, int x, int y, int w, int h, ManagedCommon::Enums::Actions::DrawingAction drawingAction)
{

	ScreenBuffer^ buffer = gcnew ScreenBuffer();
	buffer->Buffer = pictureArray;
	buffer->H = h;
	buffer->W = w;
	buffer->X = x;
	buffer->Y = y;
	buffer->DrawingAction = drawingAction;

	if (_picturesToBeSentBuffer->Count >= noUpdates)
	{
		throw gcnew Exception("Pictures to be sent buffer already is already size no updates but has not being sent");
	}

	if (OnCopyRectToScreen != nullptr)
	{
		_picturesToBeSentBuffer->Add(buffer);

		if (_picturesToBeSentBuffer->Count == noUpdates)
		{
			OnCopyRectToScreen->Invoke(_picturesToBeSentBuffer);
			_picturesToBeSentBuffer->Clear();
		}
	}
}

void CLIScumm::Wrapper::EnqueueGameEvent(IGameEvent^ gameEvent)
{
	Monitor::Enter(gameEventLock);
	gameEvent->GetType()->ToString()->Contains("Click");
	try {
		eventQueue->Enqueue(gameEvent);
	}
	finally {
		Monitor::Exit(gameEventLock);
	}
}

void CLIScumm::Wrapper::UpdatePictureBuffer(byte* pictureArray, const void* buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor* color, byte ignore)
{
	int currentPixel = 0;

	const unsigned char* bufCounter = static_cast<const unsigned char*>(buf);

	for (int heightCounter = 0; heightCounter < h; heightCounter++, bufCounter = bufCounter + pitch)
	{
		for (int widthCounter = 0; widthCounter < w; widthCounter++)
		{
			byte palletteReference = *(bufCounter + widthCounter);
			NativeScummWrapper::PalletteColor currentColor = *(color + palletteReference);

			if (ignore == 255 || palletteReference != ignore)
			{
				pictureArray[currentPixel++] = currentColor.r;
				pictureArray[currentPixel++] = currentColor.g;
				pictureArray[currentPixel++] = currentColor.b;
				pictureArray[currentPixel++] = currentColor.a;

			}
			else
			{
				for (int i = 0, wholeScreenBufferCounter = ((y + heightCounter) * DISPLAY_DEFAULT_WIDTH + x + widthCounter) * 4; i < NO_BYTES_PER_PIXEL && wholeScreenBufferCounter < _wholeScreenBufferLength; i++)
				{
					pictureArray[currentPixel++] = _wholeScreenBufferNoMouse[wholeScreenBufferCounter++];
				}
			}
		}
	}
}

void CLIScumm::Wrapper::ScreenUpdated(const void* buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor* color, byte ignore, bool isMouseUpdate, int noUpdates)
{
	byte *pictureArray = nullptr;
	try
	{
		int pictureArrayLength = w * h * NO_BYTES_PER_PIXEL;
		pictureArray = new byte[pictureArrayLength];

		UpdatePictureBuffer(pictureArray, buf, pitch, x, y, w, h, color, ignore);

		if (!isMouseUpdate) {
			UpdateWholeScreenBuffer(pictureArray, _wholeScreenBufferNoMouse, x, y, w, h);
		}

		UpdateWholeScreenBuffer(pictureArray, _wholeScreenBuffer, x, y, w, h);

		//Compress Here
		cli::array<byte> ^ managedCompressedPictureArray = gcnew array<byte>(w * h * NO_BYTES_PER_PIXEL);

		Marshal::Copy((System::IntPtr)pictureArray, managedCompressedPictureArray, 0, pictureArrayLength);

		UpdatePicturesToBeSentBuffer(managedCompressedPictureArray, noUpdates, x, y, w, h, isMouseUpdate ? ManagedCommon::Enums::Actions::DrawingAction::DrawMouse : ManagedCommon::Enums::Actions::DrawingAction::DrawPicture);
	}
	finally
	{
		delete[] pictureArray;
	}
}

void CLIScumm::Wrapper::UpdateWholeScreenBuffer(byte* pictureArray, byte* wholeScreenBuffer, int x, int y, int w, int h)
{
	try
	{
		Monitor::Enter(_wholeScreenBufferLock);
		for (int row = 0; row < h; row++)
		{
			std::memcpy(&wholeScreenBuffer[((y + row) * DISPLAY_DEFAULT_WIDTH + x) * NO_BYTES_PER_PIXEL], &pictureArray[row * w * NO_BYTES_PER_PIXEL], w * NO_BYTES_PER_PIXEL);
		}
	}
	catch (Exception^ e)
	{
		int x = 4;
	}
	finally
	{
		Monitor::Exit(_wholeScreenBufferLock);
	}
}

void CLIScumm::Wrapper::Blot(int x, int y, int w, int h, int noUpdates)
{
	byte *unCompressedPictureArray = nullptr;

	try {

		int pictureArrayLength = w * h * NO_BYTES_PER_PIXEL;
		unCompressedPictureArray = new byte[pictureArrayLength];

		for (int yCounter = 0; yCounter < h; yCounter++) {
			memcpy(&unCompressedPictureArray[yCounter * w * NO_BYTES_PER_PIXEL], &_wholeScreenBufferNoMouse[(y * DISPLAY_DEFAULT_WIDTH + x + yCounter * DISPLAY_DEFAULT_WIDTH) * NO_BYTES_PER_PIXEL], w * NO_BYTES_PER_PIXEL);
		}

		//Compression Here

		cli::array<byte> ^ pictureArray = gcnew array<byte>(w * h * NO_BYTES_PER_PIXEL);
		Marshal::Copy((System::IntPtr)unCompressedPictureArray, pictureArray, 0, pictureArrayLength);

		UpdatePicturesToBeSentBuffer(pictureArray, noUpdates, x, y, w, h, ManagedCommon::Enums::Actions::DrawingAction::Blot);
	}
	finally
	{
		if (unCompressedPictureArray != nullptr)
		{
			delete[] unCompressedPictureArray;
		}
	}
}

bool CLIScumm::Wrapper::SaveData(byte* data, int size, Common::String fileName)
{
	array<System::Byte>^ managedData = gcnew array<System::Byte>(size);
	Marshal::Copy((System::IntPtr) data, managedData, 0, size);
	return _saveData(managedData, gcnew System::String(fileName.c_str()));
}

void CLIScumm::Wrapper::InitScreen()
{
	for (int i = 0; i < _wholeScreenBufferLength; i++)
	{
		if ((i + 1) % 4 == 0)
		{
			_wholeScreenBuffer[i] = 255;
		}
		else
		{
			_wholeScreenBuffer[i] = 0;
		}
	}

	if (OnCopyRectToScreen != nullptr)
	{

		OnCopyRectToScreen->Invoke(GetListOfScreenBufferFromSinglePictureArray(MarshalWholeScreenBuffer(_wholeScreenBuffer), 0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT));
	}
}
int gameCounter = 0;
bool CLIScumm::Wrapper::pollEventWrapper(Common::Event& event)
{
	IGameEvent^ gameEvent;
	bool result = false;

	try
	{
		Monitor::Enter(gameEventLock);
		if (eventQueue->TryPeek(gameEvent))
		{
			if (gameEvent->HasEvents())
			{
				event = *((Common::Event*) gameEvent->GetEvent().ToPointer());
				result = true;

				if (!gameEvent->HasEvents())
				{
					eventQueue->TryDequeue(gameEvent);
				}
			}
		}
	}
	finally
	{
		Monitor::Exit(gameEventLock);
	}

	return result;
}

void CLIScumm::Wrapper::PlaySound(byte* buffer, int size, void* user)
{
	cli::array<byte>^ result = gcnew cli::array<byte>(size);
	Marshal::Copy((System::IntPtr) buffer, result, 0, size);

	_playAudio->Invoke(result);

	delete[] buffer;
}

byte* CLIScumm::Wrapper::GetSoundSample(byte* buffer, int size)
{
	return _gSystemCli->mixCallback(buffer, size);
}


void CLIScumm::Wrapper::Quit()
{
	IGameEvent^ gameEvent = gcnew CliScummEvents::SendQuit();

	EnqueueGameEvent(gameEvent);
}

array<Byte>^ CLIScumm::Wrapper::GetWholeScreen(int% width, int% height)
{
	if (!hasStarted)
	{
		throw gcnew Exception("Cannot get the whole screen without first starting the game");
	}

	width = DISPLAY_DEFAULT_WIDTH;
	height = DISPLAY_DEFAULT_HEIGHT;

	try
	{
		Monitor::Enter(_wholeScreenBufferLock);

		return MarshalWholeScreenBuffer(_wholeScreenBuffer);
	}
	finally
	{
		Monitor::Exit(_wholeScreenBufferLock);
	}
}
