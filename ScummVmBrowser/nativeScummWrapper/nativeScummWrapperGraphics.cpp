#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "nativeScummWrapperGraphics.h"

#define IGNORE_COLOR 1
#define DO_NOT_IGNORE_ANY_COLOR -1

NativeScummWrapper::NativeScummWrapperGraphics::NativeScummWrapperGraphics(f_SendScreenBuffers copyRect) : GraphicsManager() {
	_copyRect = copyRect;
	_picturePalette = allocatePallette();
	_cursorPalette = allocatePallette();
	_wholeScreenBuffer = new byte[WHOLE_SCREEN_BUFFER_LENGTH];
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
	delete[] _wholeScreenBuffer;
	delete[] _wholeScreenBufferNoMouse;

	CloseHandle(_wholeScreenMutex);
}

void NativeScummWrapper::NativeScummWrapperGraphics::copyRectToScreen(const void *buf, int pitch, int x, int y, int w, int h) {

	if (!_screenInited) {
		_screenInited = true;
		int _;
		byte *compressedWholeScreenBuffer = GetWholeScreenBufferRaw(_, _, _);

		ScreenBuffer initScreen = GetScreenBuffer(compressedWholeScreenBuffer, DISPLAY_DEFAULT_WIDTH, 0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT, _currentPaletteHash, false);

		_drawingCommands.push_back(initScreen);
	}

	byte* uncompressedpictureArray = ScreenUpdated(buf, pitch, x, y, w, h, false);
	_drawingCommands.push_back(GetScreenBuffer((byte *)uncompressedpictureArray, pitch, x, y, w, h, _currentPaletteHash, false));
	delete[] uncompressedpictureArray;

	if (_cliMouse.x < DISPLAY_DEFAULT_WIDTH && _cliMouse.y < DISPLAY_DEFAULT_WIDTH && _cliMouse.width > 0 && _cliMouse.height > 0) {
		if (_cliMouse.width > 0 && _cliMouse.height > 0) {
			uncompressedpictureArray = ScreenUpdated(_cliMouse.buffer, _cliMouse.fullWidth, _cliMouse.x, _cliMouse.y, _cliMouse.width, _cliMouse.height, _cursorPalette);
			_drawingCommands.push_back(GetScreenBuffer(_cliMouse.buffer, _cliMouse.fullWidth, _cliMouse.x, _cliMouse.y, _cliMouse.width, _cliMouse.height, _currentCursorPaletteHash, true));
			delete[] uncompressedpictureArray;
		}
	}
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
	//TODO: Implement
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

int counter = 0;
void NativeScummWrapper::NativeScummWrapperGraphics::updateScreen() {
	if (!_drawingCommands.empty()) {

		ScreenBuffer *buffers = new ScreenBuffer[_drawingCommands.size()];
		std::copy(_drawingCommands.begin(), _drawingCommands.end(), buffers);

		NativeScummWrapper::NativeScummWrapperGraphics::_copyRect(buffers, _drawingCommands.size());

		for (int i = 0; i < _drawingCommands.size(); i++) {
			delete[] _drawingCommands.at(i).buffer;

			if (_drawingCommands.at(i).compressedPalette != nullptr) {
				delete[] _drawingCommands.at(i).compressedPalette;
			}
		}
		delete[] buffers;
		_drawingCommands.clear();
		counter++;
	}
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
	if (positionInRange(x, y)) {
		if (_screenInited) {
			_cliMouse.x = x;
			_cliMouse.y = y;
			_cliMouse.height = restrictHeightToScreenBounds(y, _cliMouse.fullHeight);
			_cliMouse.width = restrictWidthToScreenBounds(x, _cliMouse.fullWidth);

			bool shouldBlot = positionInRange(_cliMouse.prevX, _cliMouse.prevY) && _cliMouse.prevW > 0 && _cliMouse.prevH > 0;
			bool shouldSendNewMouseExample = x < DISPLAY_DEFAULT_WIDTH && y < DISPLAY_DEFAULT_WIDTH && x < DISPLAY_DEFAULT_WIDTH && y < DISPLAY_DEFAULT_WIDTH && _cliMouse.width > 0 && _cliMouse.height > 0;

			if (shouldBlot) {
				byte *uncompressedBuffer = GetBlottedBuffer(_cliMouse.prevX, _cliMouse.prevY, _cliMouse.prevW, _cliMouse.prevH);
				_drawingCommands.push_back(GetScreenBuffer(uncompressedBuffer, _cliMouse.fullWidth, _cliMouse.prevX, _cliMouse.prevY, _cliMouse.prevW, _cliMouse.prevH, _currentPaletteHash, false));

				delete[] uncompressedBuffer;
			}

			if (shouldSendNewMouseExample) {
				byte* unCompressedPictureUpdate = ScreenUpdated(_cliMouse.buffer, _cliMouse.fullWidth, _cliMouse.x, _cliMouse.y, _cliMouse.width, _cliMouse.height, _cursorPalette);
				_drawingCommands.push_back(GetScreenBuffer(_cliMouse.buffer, _cliMouse.fullWidth, _cliMouse.x, _cliMouse.y, _cliMouse.width, _cliMouse.height, _currentCursorPaletteHash, true));
				delete unCompressedPictureUpdate;
			}
		}
		setCurrentMouseStateToPrevious();
	}
}

void NativeScummWrapper::NativeScummWrapperGraphics::setMouseCursor(const void *buf, uint w, uint h, int hotspotX, int hotspotY, uint32 keycolor, bool dontScale, const Graphics::PixelFormat *format) {
	_cliMouse.x = hotspotX;
	_cliMouse.y = hotspotY;
	_cliMouse.buffer = buf;
	_cliMouse.height = restrictHeightToScreenBounds(hotspotY, h);
	_cliMouse.width = restrictWidthToScreenBounds(hotspotX, w);
	_cliMouse.fullHeight = h;
	_cliMouse.fullWidth = w;
	_cliMouse.cursorPallette = _cursorPalette;
	_cliMouse.keyColor = keycolor;

	warpMouse(hotspotX, hotspotY);
}

void NativeScummWrapper::NativeScummWrapperGraphics::setCursorPalette(const byte *colors, uint start, uint num) {
	populatePalette(_cursorPalette, colors, start, num);

	_currentCursorPaletteHash = RememberPalette(_cursorPalette, NO_COLOURS);
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
	for (i = 0; i < num; i++, b += 3) {
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
	if (_cliMouse.y + result > DISPLAY_DEFAULT_HEIGHT) {
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
	}
}

bool NativeScummWrapper::NativeScummWrapperGraphics::screenUpdateOverlapsMouse(int x, int y, int w, int h) {
	return _cliMouse.x >= x && _cliMouse.x + _cliMouse.width <= x + w && _cliMouse.y >= y && _cliMouse.y + _cliMouse.height <= y + h;
}

bool NativeScummWrapper::NativeScummWrapperGraphics::positionInRange(int x, int y) {
	return x >= 0 && x <= DISPLAY_DEFAULT_WIDTH && y >= 0 && y <= DISPLAY_DEFAULT_HEIGHT;
}

NativeScummWrapper::MouseState NativeScummWrapper::NativeScummWrapperGraphics::getMouseState() {
	return NativeScummWrapper::MouseState(_cliMouse);
}

byte *NativeScummWrapper::NativeScummWrapperGraphics::GetBlottedBuffer(int x, int y, int w, int h) {
	byte *unCompressedPictureArray = nullptr;

	int pictureArrayLength = w * h;
	unCompressedPictureArray = new byte[pictureArrayLength];

	for (int yCounter = 0; yCounter < h; yCounter++) {
		memcpy(&unCompressedPictureArray[yCounter * w], &_wholeScreenBufferNoMouse[(y * DISPLAY_DEFAULT_WIDTH + x + yCounter * DISPLAY_DEFAULT_WIDTH)], w);
	}

	return unCompressedPictureArray;
}

byte *NativeScummWrapper::NativeScummWrapperGraphics::GetWholeScreenBufferCompressed(int &width, int &height, int &bufferSize) {
	WaitForSingleObject(_wholeScreenMutex, INFINITE);

	width = DISPLAY_DEFAULT_WIDTH;
	height = DISPLAY_DEFAULT_HEIGHT;

	byte *cpyWholeScreenBuffer = new byte[WHOLE_SCREEN_BUFFER_LENGTH];

	memcpy(cpyWholeScreenBuffer, _wholeScreenBuffer, WHOLE_SCREEN_BUFFER_LENGTH);

	ReleaseSemaphore(_wholeScreenMutex, 1, NULL);

	int compressedLength;
	byte *compressedWholeScreenBuffer = ZLibCompression::ZLibCompression().Compress(cpyWholeScreenBuffer, WHOLE_SCREEN_BUFFER_LENGTH, compressedLength);

	bufferSize = compressedLength;

	delete[] cpyWholeScreenBuffer;

	return compressedWholeScreenBuffer;
}

byte *NativeScummWrapper::NativeScummWrapperGraphics::GetWholeScreenBufferRaw(int &width, int &height, int &bufferSize) {
	byte *cpyWholeScreenBuffer = new byte[WHOLE_SCREEN_BUFFER_LENGTH];

	memcpy(cpyWholeScreenBuffer, _wholeScreenBuffer, WHOLE_SCREEN_BUFFER_LENGTH);

	width = DISPLAY_DEFAULT_WIDTH;
	height = DISPLAY_DEFAULT_HEIGHT;
	bufferSize = WHOLE_SCREEN_BUFFER_LENGTH;

	return cpyWholeScreenBuffer;
}

byte *NativeScummWrapper::NativeScummWrapperGraphics::ScreenUpdated(const void *buf, int pitch, int x, int y, int w, int h, bool isMouseUpdate) {
	byte *pictureArray = nullptr;

	int pictureArrayLength = w * h;
	pictureArray = new byte[pictureArrayLength];

	UpdatePictureBuffer(pictureArray, buf, pitch, x, y, w, h);

	if (!isMouseUpdate) {
		UpdateWholeScreenBuffer(pictureArray, _wholeScreenBufferNoMouse, x, y, w, h);
	}

	UpdateWholeScreenBuffer(pictureArray, _wholeScreenBuffer, x, y, w, h);

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
	WaitForSingleObject(_wholeScreenMutex, INFINITE);

	for (int row = 0; row < h; row++) {
		std::memcpy(&wholeScreenBuffer[((y + row) * DISPLAY_DEFAULT_WIDTH + x)], &pictureArray[row * w], w);
	}

	ReleaseSemaphore(_wholeScreenMutex, 1, NULL);
}

NativeScummWrapper::ScreenBuffer NativeScummWrapper::NativeScummWrapperGraphics::GetScreenBuffer(const void *buf, int pitch, int x, int y, int w, int h, uint32 paletteHash, bool isMouseUpdate) {
	NativeScummWrapper::ScreenBuffer screenBuffer;
	screenBuffer.buffer = (byte *)ZLibCompression::ZLibCompression().Compress((byte *)buf, w * h, screenBuffer.length);
	screenBuffer.ignoreColour = isMouseUpdate ? IGNORE_COLOR: DO_NOT_IGNORE_ANY_COLOR;
	screenBuffer.x = x;
	screenBuffer.y = y;
	screenBuffer.h = h;
	screenBuffer.w = w;
	screenBuffer.compressedPalette = nullptr;
	screenBuffer.compressedPalletteLength = 0;
	if (!palettesSeen[paletteHash]) {
		screenBuffer.compressedPalette = (byte *)ZLibCompression::ZLibCompression().Compress((byte *)palettes[paletteHash].c_str(), NO_DIGITS_IN_PALETTE_VALUE * NO_COLOURS, screenBuffer.compressedPalletteLength);
	}
	screenBuffer.paletteHash = paletteHash;
	palettesSeen[paletteHash] = true;

	return screenBuffer;
}

uint32 NativeScummWrapper::NativeScummWrapperGraphics::RememberPalette(PalletteColor *pallette, int length) {
	std::string paletteString = NativeScummWrapper::GetPalettesAsString(pallette, length);
	std::hash<std::string> str_hash;

	uint32 paletteHash = std::hash<std::string>()(paletteString);

	palettes.emplace(paletteHash, paletteString); //Doesn't matter if it already exists we are just overwritting with the same value anyway

	return paletteHash;
}

void NativeScummWrapper::NativeScummWrapperGraphics::InitScreen() {
	memset(_wholeScreenBuffer, 0, WHOLE_SCREEN_BUFFER_LENGTH);
	memset(_wholeScreenBufferNoMouse, 0, WHOLE_SCREEN_BUFFER_LENGTH);
}
