#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "nativeScummWrapperGraphics.h"

#define DISPLAY_DEFAULT_WIDTH 320
#define DISPLAY_DEFAULT_HEIGHT 200
#define NO_COLOURS 256
#define IGNORE_COLOR 255

NativeScummWrapper::NativeScummWrapperGraphics::NativeScummWrapperGraphics(f_SendScreenBuffers copyRect) : GraphicsManager() {
	_copyRect = copyRect;
	_picturePalette = allocatePallette();
	_cursorPalette = allocatePallette();
	_wholeScreenBuffer = new byte[DISPLAY_DEFAULT_WIDTH * DISPLAY_DEFAULT_HEIGHT * NO_BYTES_PER_PIXEL];
	_wholeScreenBufferNoMouse = new byte[WHOLE_SCREEN_BUFFER_LENGTH];

	_wholeScreenMutex = CreateSemaphore( //Cannot use 'std::mutex due to an iteration issue with CLR
	    NULL,                            // default security attributes
	    1,                               // initial count
	    1,                               // maximum count
	    NULL);                           // unnamed semaphore

	InitScreen();
}

NativeScummWrapper::NativeScummWrapperGraphics::~NativeScummWrapperGraphics() {
	delete[] _wholeScreenBuffer;
	delete[] _wholeScreenBufferNoMouse;

	CloseHandle(_wholeScreenMutex);
}

void NativeScummWrapper::NativeScummWrapperGraphics::copyRectToScreen(const void *buf, int pitch, int x, int y, int w, int h) {

	std::vector<ScreenBuffer> buffersVector;


	if (!_screenInited) {
		_screenInited = true;
		int _;
		byte *wholeScreenBufferCpy = GetWholeScreenBuffer(_, _, _);

		ScreenBuffer initScreen = GetScreenBuffer(wholeScreenBufferCpy, 0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT);

		buffersVector.push_back(initScreen);
	}

	buffersVector.push_back(GetScreenBuffer(ScreenUpdated(buf, DISPLAY_DEFAULT_WIDTH, x, y, w, h, _picturePalette, IGNORE_COLOR, false), x, y, w, h));

	if (_cliMouse.x < DISPLAY_DEFAULT_WIDTH && _cliMouse.y < DISPLAY_DEFAULT_WIDTH && _cliMouse.width > 0 && _cliMouse.height > 0) {
		if (_cliMouse.width > 0 && _cliMouse.height > 0) {
			buffersVector.push_back(GetScreenBuffer(ScreenUpdated(_cliMouse.buffer, _cliMouse.fullWidth, _cliMouse.x, _cliMouse.y, _cliMouse.width, _cliMouse.height, _cursorPalette, _cliMouse.keyColor, true), _cliMouse.x, _cliMouse.y, _cliMouse.width, _cliMouse.height));
		}
	}

	ScreenBuffer *buffers = new ScreenBuffer[buffersVector.size()];
	std::copy(buffersVector.begin(), buffersVector.end(), buffers);

	NativeScummWrapper::NativeScummWrapperGraphics::_copyRect(buffers, buffersVector.size());

	for (int i = 0; i < buffersVector.size(); i++) {
		delete[] buffersVector.at(i).buffer;
	}

	delete[] buffers;
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

			int noMessages = 0;

			_cliMouse.x = x;
			_cliMouse.y = y;
			_cliMouse.height = restrictHeightToScreenBounds(y, _cliMouse.fullHeight);
			_cliMouse.width = restrictWidthToScreenBounds(x, _cliMouse.fullWidth);

			bool shouldBlot = positionInRange(_cliMouse.prevX, _cliMouse.prevY) && _cliMouse.width > 0 && _cliMouse.height > 0;
			bool shouldSendNewMouseExample = x < DISPLAY_DEFAULT_WIDTH && y < DISPLAY_DEFAULT_WIDTH && x < DISPLAY_DEFAULT_WIDTH && y < DISPLAY_DEFAULT_WIDTH && _cliMouse.width > 0 && _cliMouse.height > 0;

			if (shouldBlot) {
				noMessages++;
			}

			if (shouldSendNewMouseExample) {
				noMessages++;
			}

			ScreenBuffer *screenBuffers = new ScreenBuffer[noMessages];

			if (shouldBlot) {
				screenBuffers[0] = GetScreenBuffer(GetBlottedBuffer(_cliMouse.prevX, _cliMouse.prevY, _cliMouse.prevW, _cliMouse.prevH), _cliMouse.prevX, _cliMouse.prevY, _cliMouse.prevW, _cliMouse.prevH);
			}

			if (shouldSendNewMouseExample) {
				screenBuffers[noMessages - 1] = GetScreenBuffer(_cliMouse.buffer, _cliMouse.x, _cliMouse.y, _cliMouse.width, _cliMouse.height);
			}

			if (noMessages > 0) {
				NativeScummWrapper::NativeScummWrapperGraphics::_copyRect(screenBuffers, noMessages);
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

	int pictureArrayLength = w * h * NO_BYTES_PER_PIXEL;
	unCompressedPictureArray = new byte[pictureArrayLength];

	for (int yCounter = 0; yCounter < h; yCounter++) {
		memcpy(&unCompressedPictureArray[yCounter * w * NO_BYTES_PER_PIXEL], &_wholeScreenBufferNoMouse[(y * DISPLAY_DEFAULT_WIDTH + x + yCounter * DISPLAY_DEFAULT_WIDTH) * NO_BYTES_PER_PIXEL], w * NO_BYTES_PER_PIXEL);
	}

	return unCompressedPictureArray;
}

byte* NativeScummWrapper::NativeScummWrapperGraphics::GetWholeScreenBuffer(int &width, int &height, int &bufferSize) {
	WaitForSingleObject(_wholeScreenMutex, INFINITE);

	width = DISPLAY_DEFAULT_WIDTH;
	height = DISPLAY_DEFAULT_HEIGHT;
	bufferSize = WHOLE_SCREEN_BUFFER_LENGTH;

	byte *cpyWholeScreenBuffer = new byte[WHOLE_SCREEN_BUFFER_LENGTH];

	memcpy(cpyWholeScreenBuffer, _wholeScreenBuffer, WHOLE_SCREEN_BUFFER_LENGTH);

	ReleaseSemaphore(_wholeScreenMutex, 1, NULL);

	return cpyWholeScreenBuffer;
}

byte* NativeScummWrapper::NativeScummWrapperGraphics::ScreenUpdated(const void *buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor *color, byte ignore, bool isMouseUpdate) {
	byte *pictureArray = nullptr;

	int pictureArrayLength = w * h * NO_BYTES_PER_PIXEL;
	pictureArray = new byte[pictureArrayLength];

	UpdatePictureBuffer(pictureArray, buf, pitch, x, y, w, h, color, ignore);

	if (!isMouseUpdate) {
		UpdateWholeScreenBuffer(pictureArray, _wholeScreenBufferNoMouse, x, y, w, h);
	}

	UpdateWholeScreenBuffer(pictureArray, _wholeScreenBuffer, x, y, w, h);

	return pictureArray;
}

void NativeScummWrapper::NativeScummWrapperGraphics::UpdatePictureBuffer(byte *pictureArray, const void *buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor *color, byte ignore) {
	int currentPixel = 0;

	const unsigned char *bufCounter = static_cast<const unsigned char *>(buf);

	for (int heightCounter = 0; heightCounter < h; heightCounter++, bufCounter = bufCounter + pitch) {
		for (int widthCounter = 0; widthCounter < w; widthCounter++) {
			byte palletteReference = *(bufCounter + widthCounter);
			NativeScummWrapper::PalletteColor currentColor = *(color + palletteReference);

			if (ignore == 255 || palletteReference != ignore) {
				pictureArray[currentPixel++] = currentColor.r;
				pictureArray[currentPixel++] = currentColor.g;
				pictureArray[currentPixel++] = currentColor.b;
				pictureArray[currentPixel++] = currentColor.a;

			} else {
				for (int i = 0, wholeScreenBufferCounter = ((y + heightCounter) * DISPLAY_DEFAULT_WIDTH + x + widthCounter) * 4; i < NO_BYTES_PER_PIXEL && wholeScreenBufferCounter < WHOLE_SCREEN_BUFFER_LENGTH; i++) {
					pictureArray[currentPixel++] = _wholeScreenBufferNoMouse[wholeScreenBufferCounter++];
				}
			}
		}
	}
}

void NativeScummWrapper::NativeScummWrapperGraphics::UpdateWholeScreenBuffer(byte *pictureArray, byte *wholeScreenBuffer, int x, int y, int w, int h) {
	WaitForSingleObject(_wholeScreenMutex, INFINITE);

	for (int row = 0; row < h; row++) {
		std::memcpy(&wholeScreenBuffer[((y + row) * DISPLAY_DEFAULT_WIDTH + x) * NO_BYTES_PER_PIXEL], &pictureArray[row * w * NO_BYTES_PER_PIXEL], w * NO_BYTES_PER_PIXEL);
	}

	ReleaseSemaphore(_wholeScreenMutex, 1, NULL);
}

NativeScummWrapper::ScreenBuffer NativeScummWrapper::NativeScummWrapperGraphics::GetScreenBuffer(const void *buf, int x, int y, int w, int h) {
	NativeScummWrapper::ScreenBuffer screenBuffer;
	screenBuffer.buffer = (byte *)buf;
	screenBuffer.x = x;
	screenBuffer.y = y;
	screenBuffer.h = h;
	screenBuffer.w = w;

	return screenBuffer;
}

void NativeScummWrapper::NativeScummWrapperGraphics::InitScreen() {
	for (int i = 0; i < WHOLE_SCREEN_BUFFER_LENGTH; i++) {
		if ((i + 1) % 4 == 0) {
			_wholeScreenBuffer[i] = 255;
			_wholeScreenBufferNoMouse[i] = 255;
		} else {
			_wholeScreenBuffer[i] = 0;
			_wholeScreenBufferNoMouse[i] = 0;
		}
	}
}
