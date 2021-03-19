#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "nativeScummWrapperGraphics.h"
#include "C:\scumm\ScummVmBrowser\LaunchDebugger\LaunchDebugger.h"
#define DO_NOT_IGNORE_ANY_COLOR -1

NativeScummWrapper::NativeScummWrapperGraphics::NativeScummWrapperGraphics(f_SendScreenBuffers copyRect) : GraphicsManager() {
	_copyRect = copyRect;
	_picturePalette = allocatePallette();
	_cursorPalette = allocatePallette();

	_wholeScreenBufferNoMouse = new byte[WHOLE_SCREEN_BUFFER_LENGTH];

	_wholeScreenMutex = CreateSemaphore( //Cannot use 'std::mutex due to an iteration issue with CLR
	    NULL,                            // default security attributes
	    1,                               // initial count
	    1,                               // maximum count
	    NULL);                           // unnamed semaphore
	_currentPaletteHash = 0;
	_currentCursorPaletteHash = 0;
	InitScreen();
}

NativeScummWrapper::NativeScummWrapperGraphics::~NativeScummWrapperGraphics() {
	delete[] _wholeScreenBufferNoMouse;
	free(_picturePalette);
	free(_cursorPalette);
	CloseHandle(_wholeScreenMutex);
}

void NativeScummWrapper::NativeScummWrapperGraphics::copyRectToScreen(const void *buf, int pitch, int x, int y, int w, int h) {
	WaitForSingleObject(_wholeScreenMutex, INFINITE);

	if (!_screenInited) {
		_screenInited = true;
		int _;
		byte *wholeScreenBuffer = GetWholeScreenBufferRaw(_, _, _);

		ScreenBuffer initScreen = GetScreenBuffer(wholeScreenBuffer, DISPLAY_DEFAULT_WIDTH, 0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT, _currentPaletteHash, false, false);

		_drawingBuffers.push_back(initScreen);
	}

	bool differenceDetected;
	byte *pictureBuffer = ScreenUpdated(buf, pitch, x, y, w, h, false, differenceDetected);

	if (differenceDetected) {
		_drawingBuffers.push_back(GetScreenBuffer((byte *)pictureBuffer, pitch, x, y, w, h, _currentPaletteHash, false, false));

		if (_cliMouse.adjustedX() < DISPLAY_DEFAULT_WIDTH && _cliMouse.adjustedY() < DISPLAY_DEFAULT_HEIGHT && _cliMouse.width > 0 && _cliMouse.height > 0 && screenUpdateOverlapsMouse(x, y, w, h)) {
			if (_cliMouse.width > 0 && _cliMouse.height > 0) {
				_drawingBuffers.push_back(GetMouseScreenBuffer(false));
			}
		}
	} else {
		delete[] pictureBuffer;
	}

	ReleaseSemaphore(_wholeScreenMutex, 1, NULL);
}

bool NativeScummWrapper::NativeScummWrapperGraphics::hasFeature(OSystem::Feature f) const {
	switch (f) {
	case OSystem::Feature::kFeatureCursorPalette:
		return true;
	}
	return false;
	//TODO: Implement
}

void NativeScummWrapper::NativeScummWrapperGraphics::setFeatureState(OSystem::Feature f, bool enable) {
	switch (f) {
	case OSystem::kFeatureCursorPalette:
		_cursorPaletteDisabled = !enable;
		break;
	default:
		break;
	}
}

bool NativeScummWrapper::NativeScummWrapperGraphics::getFeatureState(OSystem::Feature f) const {
	//TODO: Implement
	return false;
}

const OSystem::GraphicsMode *NativeScummWrapper::NativeScummWrapperGraphics::getSupportedGraphicsModes() const {
	//TODO: Implement
	return nullptr;
}

int NativeScummWrapper::NativeScummWrapperGraphics::getDefaultGraphicsMode() const {
	//TODO: Implement
	return 0;
}

bool NativeScummWrapper::NativeScummWrapperGraphics::setGraphicsMode(int mode) {
	//TODO: Implement
	return false;
}

void NativeScummWrapper::NativeScummWrapperGraphics::resetGraphicsScale() {
	//TODO: Implement
}

int NativeScummWrapper::NativeScummWrapperGraphics::getGraphicsMode() const {
	//TODO: Implement
	return 0;
}

//TO DO: Implement All These
void NativeScummWrapper::NativeScummWrapperGraphics::initSize(uint width, uint height, const Graphics::PixelFormat *format) {
}

int NativeScummWrapper::NativeScummWrapperGraphics::getScreenChangeID() const {
	return 0;
}

void NativeScummWrapper::NativeScummWrapperGraphics::beginGFXTransaction() {
}

OSystem::TransactionError NativeScummWrapper::NativeScummWrapperGraphics::endGFXTransaction() {
	return OSystem::TransactionError();
}

int16 NativeScummWrapper::NativeScummWrapperGraphics::getHeight() const {
	return DISPLAY_DEFAULT_HEIGHT; //TODO: Fix Hack
}

int16 NativeScummWrapper::NativeScummWrapperGraphics::getWidth() const {
	return DISPLAY_DEFAULT_WIDTH; //TODO: Fix Hack
}

void NativeScummWrapper::NativeScummWrapperGraphics::setPalette(const byte *colors, uint start, uint num) {
	populatePalette(_picturePalette, colors, start, num);

	_currentPaletteHash = RememberPalette(_picturePalette, NO_COLOURS);

	if (_cursorPaletteDisabled)
	{
		_currentCursorPaletteHash = _currentPaletteHash;
	}

	ScheduleRedrawWholeScreen();
}

void NativeScummWrapper::NativeScummWrapperGraphics::grabPalette(byte *colors, uint start, uint num) const {
}

Graphics::Surface *NativeScummWrapper::NativeScummWrapperGraphics::lockScreen() {
	return nullptr;
}

void NativeScummWrapper::NativeScummWrapperGraphics::unlockScreen() {
}

void NativeScummWrapper::NativeScummWrapperGraphics::fillScreen(uint32 col) {
}


void NativeScummWrapper::NativeScummWrapperGraphics::updateScreen() {
	WaitForSingleObject(_wholeScreenMutex, INFINITE);

	if (!_drawingBuffers.empty()) {
		NativeScummWrapper::NativeScummWrapperGraphics::_copyRect(&_drawingBuffers[0], _drawingBuffers.size());

		for (int i = 0; i < _drawingBuffers.size(); i++) {
				delete[] _drawingBuffers.at(i).buffer;
			}
		_drawingBuffers.clear();
	}

	ReleaseSemaphore(_wholeScreenMutex, 1, NULL);
}

void NativeScummWrapper::NativeScummWrapperGraphics::setShakePos(int shakeXOffset, int shakeYOffset) {
}

void NativeScummWrapper::NativeScummWrapperGraphics::setFocusRectangle(const Common::Rect &rect) {
}

void NativeScummWrapper::NativeScummWrapperGraphics::clearFocusRectangle() {
}

void NativeScummWrapper::NativeScummWrapperGraphics::showOverlay() {
}

void NativeScummWrapper::NativeScummWrapperGraphics::hideOverlay() {
}

Graphics::PixelFormat NativeScummWrapper::NativeScummWrapperGraphics::getOverlayFormat() const {
	Graphics::PixelFormat pixelFormat = Graphics::PixelFormat();
	pixelFormat.bytesPerPixel = 4; //TO DO FIX THIS

	return pixelFormat;
}

void NativeScummWrapper::NativeScummWrapperGraphics::clearOverlay() {
}

void NativeScummWrapper::NativeScummWrapperGraphics::grabOverlay(void *buf, int pitch) const {
}

void NativeScummWrapper::NativeScummWrapperGraphics::copyRectToOverlay(const void *buf, int pitch, int x, int y, int w, int h) {
}

int16 NativeScummWrapper::NativeScummWrapperGraphics::getOverlayHeight() const {
	return DISPLAY_DEFAULT_HEIGHT; //TODO: Fix Hack
}

int16 NativeScummWrapper::NativeScummWrapperGraphics::getOverlayWidth() const {
	return DISPLAY_DEFAULT_WIDTH; //TODO: Fix Hack
}

bool NativeScummWrapper::NativeScummWrapperGraphics::showMouse(bool visible) {
	return visible;
}

void NativeScummWrapper::NativeScummWrapperGraphics::warpMouse(int x, int y) {
	WaitForSingleObject(_wholeScreenMutex, INFINITE);

	if (positionInRange(x - _cliMouse.hotX, y - _cliMouse.hotY)) {
		if (_screenInited) {
			_cliMouse.x = x;
			_cliMouse.y = y;
			_cliMouse.width = restrictWidthToScreenBounds(_cliMouse.adjustedX(), _cliMouse.fullWidth);
			_cliMouse.height = restrictHeightToScreenBounds(_cliMouse.adjustedY(), _cliMouse.fullHeight);
			

			bool shouldBlot = positionInRange(_cliMouse.adjustedPrevX(), _cliMouse.adjustedPrevY()) && _cliMouse.prevW > 0 && _cliMouse.prevH > 0;
			bool shouldSendNewMouseExample = _cliMouse.adjustedX() < DISPLAY_DEFAULT_WIDTH && _cliMouse.adjustedY() < DISPLAY_DEFAULT_WIDTH && _cliMouse.adjustedX() < DISPLAY_DEFAULT_WIDTH && _cliMouse.adjustedY() < DISPLAY_DEFAULT_WIDTH && _cliMouse.width > 0 && _cliMouse.height > 0;

			if (shouldBlot) {
				byte *blotBuffer = GetBlottedBuffer(_cliMouse.adjustedPrevX(), _cliMouse.adjustedPrevY(), _cliMouse.prevW, _cliMouse.prevH);
				_drawingBuffers.push_back(GetScreenBuffer(blotBuffer, _cliMouse.fullWidth, _cliMouse.adjustedPrevX(), _cliMouse.adjustedPrevY(), _cliMouse.prevW, _cliMouse.prevH, _currentPaletteHash, false, false));
			}

			if (shouldSendNewMouseExample) {
				_drawingBuffers.push_back(GetMouseScreenBuffer(false));
			}
		}
		setCurrentMouseStateToPrevious();

	}

    ReleaseSemaphore(_wholeScreenMutex, 1, NULL);
}

void NativeScummWrapper::NativeScummWrapperGraphics::setMouseCursor(const void *buf, uint w, uint h, int hotspotX, int hotspotY, uint32 keycolor, bool dontScale, const Graphics::PixelFormat *format) {
	_cliMouse.hotX = hotspotX;
	_cliMouse.hotY = hotspotY;
	_cliMouse.buffer = buf;
	_cliMouse.height = restrictHeightToScreenBounds(hotspotY, h);
	_cliMouse.width = restrictWidthToScreenBounds(hotspotX, w);
	_cliMouse.fullHeight = h;
	_cliMouse.fullWidth = w;
	_cliMouse.cursorPallette = _cursorPaletteDisabled ? _picturePalette : _cursorPalette;
	_cliMouse.keyColor = keycolor;

	warpMouse(_cliMouse.x, _cliMouse.y);
}

void NativeScummWrapper::NativeScummWrapperGraphics::setCursorPalette(const byte *colors, uint start, uint num) {
	populatePalette(_cursorPalette, colors, start, num);

	_currentCursorPaletteHash = RememberPalette(_cursorPalette, NO_COLOURS);
	_cursorPaletteDisabled = false;
}

Graphics::PixelFormat NativeScummWrapper::NativeScummWrapperGraphics::getScreenFormat() const {
	return Graphics::PixelFormat();
}

Common::List<Graphics::PixelFormat> NativeScummWrapper::NativeScummWrapperGraphics::getSupportedFormats() const {
	return Common::List<Graphics::PixelFormat>();
}

NativeScummWrapper::PalletteColor *NativeScummWrapper::NativeScummWrapperGraphics::allocatePallette() {
	return (NativeScummWrapper::PalletteColor *)calloc(sizeof(NativeScummWrapper::PalletteColor), NO_COLOURS);
}

void NativeScummWrapper::NativeScummWrapperGraphics::populatePalette(NativeScummWrapper::PalletteColor *pallette, const byte *colors, uint start, uint num) {
	assert(colors);

	// Setting the palette before _screen is created is allowed - for now -
	// since we don't actually set the palette until the screen is updated.
	// But it could indicate a programming error, so let's warn about it.

	const byte *b = colors;
	uint i;
	NativeScummWrapper::PalletteColor *base = pallette + start;
	for (i = 0; i < num; i++, b += NO_COLOUR_COMPONENTS_SCUMM_VM) {
		base[i].r = b[0];
		base[i].g = b[1];
		base[i].b = b[2];
		base[i].a = 255;
	}
}

int NativeScummWrapper::NativeScummWrapperGraphics::restrictWidthToScreenBounds(int x, int width) {
	int result = width;

	if (x + width > DISPLAY_DEFAULT_WIDTH) {
		result = (result - ((x + result) - DISPLAY_DEFAULT_WIDTH)) - 1;
	}

	if (result < 0) {
		result = 0;
	}

	return result;
}

int NativeScummWrapper::NativeScummWrapperGraphics::restrictHeightToScreenBounds(int y, int height) {
	int result = height;
	if (y + result > DISPLAY_DEFAULT_HEIGHT) {
		result = (result - ((y + height) - DISPLAY_DEFAULT_HEIGHT)) - 1;
	}

	if (result < 0) {
		result = 0;
	}

	return result;
}

void NativeScummWrapper::NativeScummWrapperGraphics::setCurrentMouseStateToPrevious() {
	if (_cliMouse.height != -1) {
		_cliMouse.prevH = _cliMouse.height;
		_cliMouse.prevW = _cliMouse.width;
		_cliMouse.prevX = _cliMouse.x;
		_cliMouse.prevY = _cliMouse.y;
		_cliMouse.prevHotX = _cliMouse.hotX;
		_cliMouse.prevHotY = _cliMouse.hotY;
	}
}

bool NativeScummWrapper::NativeScummWrapperGraphics::screenUpdateOverlapsMouse(int x, int y, int w, int h) {
	return _cliMouse.adjustedX() + _cliMouse.width >= x && _cliMouse.adjustedX() <= x + w && _cliMouse.adjustedY() + _cliMouse.height >= y && _cliMouse.adjustedY() <= y + h;
}

bool NativeScummWrapper::NativeScummWrapperGraphics::positionInRange(int x, int y) {
	return x >= 0 && x <= DISPLAY_DEFAULT_WIDTH && y >= 0 && y <= DISPLAY_DEFAULT_HEIGHT;
}

NativeScummWrapper::MouseState NativeScummWrapper::NativeScummWrapperGraphics::getMouseState() {
	return NativeScummWrapper::MouseState(_cliMouse);
}

byte *NativeScummWrapper::NativeScummWrapperGraphics::GetBlottedBuffer(int x, int y, int w, int h) {
	byte *pictureBuffer = nullptr;

	int pictureArrayLength = w * h;
	pictureBuffer = new byte[pictureArrayLength];

	for (int yCounter = 0; yCounter < h; yCounter++) {
		memcpy(&pictureBuffer[yCounter * w], &_wholeScreenBufferNoMouse[(y * DISPLAY_DEFAULT_WIDTH + x + yCounter * DISPLAY_DEFAULT_WIDTH)], w);
	}

	return pictureBuffer;
}

void NativeScummWrapper::NativeScummWrapperGraphics::ScheduleRedrawWholeScreen() {
	WaitForSingleObject(_wholeScreenMutex, INFINITE);

	byte *cpyWholeScreenBuffer = new byte[WHOLE_SCREEN_BUFFER_LENGTH];

	memcpy(cpyWholeScreenBuffer, _wholeScreenBufferNoMouse, WHOLE_SCREEN_BUFFER_LENGTH);

	_drawingBuffers.push_back(GetScreenBuffer(cpyWholeScreenBuffer, DISPLAY_DEFAULT_WIDTH, 0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT, _currentPaletteHash, false, true));
	_drawingBuffers.push_back(GetMouseScreenBuffer(true));

	ReleaseSemaphore(_wholeScreenMutex, 1, NULL);
}

byte *NativeScummWrapper::NativeScummWrapperGraphics::GetWholeScreenBufferRaw(int &width, int &height, int &bufferSize) {
	byte *cpyWholeScreenBuffer = new byte[WHOLE_SCREEN_BUFFER_LENGTH];

	memcpy(cpyWholeScreenBuffer, _wholeScreenBufferNoMouse, WHOLE_SCREEN_BUFFER_LENGTH);

	width = DISPLAY_DEFAULT_WIDTH;
	height = DISPLAY_DEFAULT_HEIGHT;
	bufferSize = WHOLE_SCREEN_BUFFER_LENGTH;

	return cpyWholeScreenBuffer;
}

byte *NativeScummWrapper::NativeScummWrapperGraphics::ScreenUpdated(const void *buf, int pitch, int x, int y, int w, int h, bool isMouseUpdate, bool& differenceDetected) {
	byte *pictureArray = nullptr;

	int pictureArrayLength = w * h;

	if (pictureArrayLength > 0) {

		pictureArray = new byte[pictureArrayLength];

		UpdatePictureBuffer(pictureArray, buf, pitch, x, y, w, h);

		differenceDetected = IsScreenUpdateRequired(pictureArray, x, y, w, h);

		if (!isMouseUpdate) {
			UpdateWholeScreenBuffer(pictureArray, _wholeScreenBufferNoMouse, x, y, w, h);
		}
	}

	return pictureArray;
}

void NativeScummWrapper::NativeScummWrapperGraphics::UpdatePictureBuffer(byte *pictureArray, const void *buf, int pitch, int x, int y, int w, int h) {
	int currentPixel = 0;

	const unsigned char *bufCounter = static_cast<const unsigned char *>(buf);

	for (int heightCounter = 0; heightCounter < h; heightCounter++, bufCounter = bufCounter + pitch) {
		for (int widthCounter = 0; widthCounter < w; widthCounter++) {
			byte palletteReference = *(bufCounter + widthCounter);
			pictureArray[currentPixel++] = palletteReference;
		}

		//memcpy(pictureArray + (heightCounter * pitch), bufCounter, w);
	}
}

void NativeScummWrapper::NativeScummWrapperGraphics::UpdateWholeScreenBuffer(byte *pictureArray, byte *wholeScreenBuffer, int x, int y, int w, int h) {
	for (int row = 0; row < h; row++) {
		std::memcpy(&wholeScreenBuffer[((y + row) * DISPLAY_DEFAULT_WIDTH + x)], &pictureArray[row * w], w);
	}
}

bool NativeScummWrapper::NativeScummWrapperGraphics::IsScreenUpdateRequired(byte *pictureArray, int x, int y, int w, int h) {
	bool noDifference = true;
	for (int row = 0; row < h && noDifference; row++) {
		for (int widthCounter = 0; widthCounter < w && noDifference; widthCounter++) {
			noDifference = _wholeScreenBufferNoMouse[((y + row) * DISPLAY_DEFAULT_WIDTH + x + widthCounter)] == pictureArray[row * w + widthCounter];
		}
	}

	return !noDifference;
}

NativeScummWrapper::ScreenBuffer NativeScummWrapper::NativeScummWrapperGraphics::GetScreenBuffer(const void *buf, int pitch, int x, int y, int w, int h, uint32 paletteHash, bool isMouseUpdate, bool forcePaletteToBeSent) {
	NativeScummWrapper::ScreenBuffer screenBuffer;
	screenBuffer.buffer = (byte *)buf;
	screenBuffer.length = w * h;
	screenBuffer.ignoreColour = isMouseUpdate ? _cliMouse.keyColor : DO_NOT_IGNORE_ANY_COLOR;
	screenBuffer.x = x;
	screenBuffer.y = y;
	screenBuffer.h = h;
	screenBuffer.w = w;
	screenBuffer.paletteBuffer = nullptr;
	screenBuffer.paletteBufferLength = 0;

	if (!palettesSeen[paletteHash] || forcePaletteToBeSent) {
		screenBuffer.paletteBuffer = (byte *)palettes[paletteHash].c_str();
		screenBuffer.paletteBufferLength = NO_DIGITS_IN_PALETTE_VALUE * NO_COLOURS * NO_BYTES_PER_PIXEL;
	}
	screenBuffer.paletteHash = paletteHash;
	palettesSeen[paletteHash] = true;

	return screenBuffer;
}

NativeScummWrapper::ScreenBuffer NativeScummWrapper::NativeScummWrapperGraphics::GetMouseScreenBuffer(bool forcePalettesToBeSent) {
	ScreenBuffer result;
	bool _;

	if (_cliMouse.hasInited() && positionInRange(_cliMouse.adjustedX(), _cliMouse.adjustedY()) && _cliMouse.width > 0 && _cliMouse.height > 0) {
		byte *pictureBuffer = ScreenUpdated(_cliMouse.buffer, _cliMouse.fullWidth, _cliMouse.adjustedX(), _cliMouse.adjustedY(), _cliMouse.width, _cliMouse.height, true, _);
		result = GetScreenBuffer(pictureBuffer, _cliMouse.fullWidth, _cliMouse.adjustedX(), _cliMouse.adjustedY(), _cliMouse.width, _cliMouse.height, _currentCursorPaletteHash, true, forcePalettesToBeSent);
	}
	else {
		byte *pictureBuffer = new byte[0];
		result = GetScreenBuffer(pictureBuffer, _cliMouse.fullWidth, 0, 0, 0, 0, _currentCursorPaletteHash, true, forcePalettesToBeSent);
	}
	return result;
}


uint32 NativeScummWrapper::NativeScummWrapperGraphics::RememberPalette(PalletteColor *pallette, int length) {
	std::string paletteString = NativeScummWrapper::GetPalettesAsString(pallette, length);
	std::hash<std::string> str_hash;

	uint32 paletteHash = std::hash<std::string>()(paletteString);

	palettes.emplace(paletteHash, paletteString); //Doesn't matter if it already exists we are just overwritting with the same value anyway

	return paletteHash;
}

void NativeScummWrapper::NativeScummWrapperGraphics::InitScreen() {
	memset(_wholeScreenBufferNoMouse, 0, WHOLE_SCREEN_BUFFER_LENGTH);
	byte colours[NO_COLOURS * NO_COLOUR_COMPONENTS_SCUMM_VM];

	memset(colours, 0, NO_COLOURS * NO_COLOUR_COMPONENTS_SCUMM_VM);

		populatePalette(_picturePalette, colours, 0, NO_COLOURS);
	_currentPaletteHash = RememberPalette(_picturePalette, NO_COLOURS);
	populatePalette(_cursorPalette, colours, 0, NO_COLOURS);
	_currentCursorPaletteHash = RememberPalette(_cursorPalette, NO_COLOURS);
}
