#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "PaletteManager.h"

NativeScummWrapper::nativeScummWrapperPaletteManager::nativeScummWrapperPaletteManager() {
	_picturePalette = allocatePallette();
	_cursorPalette = allocatePallette();
	_pictureColor = new byte[NO_COLOURS * NO_COLOUR_COMPONENTS_SCUMM_VM];

	memset(_pictureColor, 0, NO_COLOURS * NO_COLOUR_COMPONENTS_SCUMM_VM);

	populatePalette(_picturePalette, _pictureColor, 0, NO_COLOURS);
	int32 paletteHash = RememberPalette(_picturePalette, NO_COLOURS);
	populatePalette(_cursorPalette, _pictureColor, 0, NO_COLOURS);
	_currentPaletteHash = RememberPalette(_picturePalette, NO_COLOURS);
	_currentCursorPaletteHash = RememberPalette(_cursorPalette, NO_COLOURS);
}

NativeScummWrapper::nativeScummWrapperPaletteManager::~nativeScummWrapperPaletteManager() {
	free(_picturePalette);
	free(_cursorPalette);

	if (_pictureColor) {
		delete[] _pictureColor;
	}
}

NativeScummWrapper::PalletteColor* NativeScummWrapper::nativeScummWrapperPaletteManager::allocatePallette() {
	return (NativeScummWrapper::PalletteColor *)calloc(sizeof(NativeScummWrapper::PalletteColor), NO_COLOURS);
}

uint32 NativeScummWrapper::nativeScummWrapperPaletteManager::RememberPalette(NativeScummWrapper::PalletteColor *palette, int length) {
	std::string paletteString = NativeScummWrapper::GetPalettesAsString(palette, length);
	std::hash<std::string> str_hash;

	uint32 paletteHash = std::hash<std::string>()(paletteString);

	palettes.emplace(paletteHash, paletteString); //Doesn't matter if it already exists we are just overwritting with the same value anyway

	return paletteHash;
}

uint32 NativeScummWrapper::nativeScummWrapperPaletteManager::populatePalette(NativeScummWrapper::PalletteColor *palette, const byte *colors, uint start, uint num) {
	assert(colors);

	// Setting the palette before _screen is created is allowed - for now -
	// since we don't actually set the palette until the screen is updated.
	// But it could indicate a programming error, so let's warn about it.

	const byte *b = colors;
	uint i;
	NativeScummWrapper::PalletteColor *base = palette + start;
	for (i = 0; i < num; i++, b += NO_COLOUR_COMPONENTS_SCUMM_VM) {
		base[i].r = b[0];
		base[i].g = b[1];
		base[i].b = b[2];
		base[i].a = 255;
	}

	return RememberPalette(palette, num - start);
}

uint32 NativeScummWrapper::nativeScummWrapperPaletteManager::getCurrentPaletteHash() {
	return _currentPaletteHash;
}

void NativeScummWrapper::nativeScummWrapperPaletteManager::setCurrentPaletteHash(uint32 value) {
	_currentPaletteHash = value;
}

uint32 NativeScummWrapper::nativeScummWrapperPaletteManager::getCurrentCursorPaletteHash() {
	if (!_cursorPaletteDisabled) {
		return _currentCursorPaletteHash;
	}
	else {
		return _currentPaletteHash;
	}

}

void NativeScummWrapper::nativeScummWrapperPaletteManager::setCurrentCursorPaletteHash(uint32 value) {
	_currentCursorPaletteHash = value;
}

uint32 NativeScummWrapper::nativeScummWrapperPaletteManager::populatePicturePalette(const byte *colors, uint start, uint num) {
	return populatePalette(_picturePalette, colors, start, num);
}

uint32 NativeScummWrapper::nativeScummWrapperPaletteManager::populateCursorPalette(const byte *colors, uint start, uint num) {
	return populatePalette(_cursorPalette, colors, start, num);
}

void NativeScummWrapper::nativeScummWrapperPaletteManager::setCursorPaletteDisabled(bool value) {
	_cursorPaletteDisabled = value;
}

void NativeScummWrapper::nativeScummWrapperPaletteManager::grabPalette(byte *colors, uint start, uint num) const {
	memcpy(colors, _pictureColor + start, num);
}

bool NativeScummWrapper::nativeScummWrapperPaletteManager::haveSeenPalette(uint32 paletteHash) {
	return palettesSeen[paletteHash];
}

void NativeScummWrapper::nativeScummWrapperPaletteManager::registerSeenPalette(uint32 paletteHash) {
	palettesSeen[paletteHash] = true;
}

const char* NativeScummWrapper::nativeScummWrapperPaletteManager::getPalette(uint32 paletteHash) {
	return palettes[paletteHash].c_str();
}
