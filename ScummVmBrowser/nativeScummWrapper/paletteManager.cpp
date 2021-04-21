#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "PaletteManager.h"

NativeScummWrapper::NativeScummWrapperPaletteManager::NativeScummWrapperPaletteManager() {
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

NativeScummWrapper::NativeScummWrapperPaletteManager::~NativeScummWrapperPaletteManager() {
	free(_picturePalette);
	free(_cursorPalette);

	if (_pictureColor) {
		delete[] _pictureColor;
	}
}

NativeScummWrapper::PalletteColor* NativeScummWrapper::NativeScummWrapperPaletteManager::allocatePallette() {
	return (NativeScummWrapper::PalletteColor *)calloc(sizeof(NativeScummWrapper::PalletteColor), NO_COLOURS);
}

uint32 NativeScummWrapper::NativeScummWrapperPaletteManager::RememberPalette(NativeScummWrapper::PalletteColor *palette, int length) {
	std::string paletteString = NativeScummWrapper::GetPalettesAsString(palette, length);
	std::hash<std::string> str_hash;

	uint32 paletteHash = std::hash<std::string>()(paletteString);

	palettes.emplace(paletteHash, paletteString); //Doesn't matter if it already exists we are just overwritting with the same value anyway

	return paletteHash;
}

uint32 NativeScummWrapper::NativeScummWrapperPaletteManager::populatePalette(NativeScummWrapper::PalletteColor *palette, const byte *colors, uint start, uint num) {
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

	return RememberPalette(palette, NO_COLOURS);
}

uint32 NativeScummWrapper::NativeScummWrapperPaletteManager::getCurrentPaletteHash() {
	return _currentPaletteHash;
}

NativeScummWrapper::PalletteColor* NativeScummWrapper::NativeScummWrapperPaletteManager::getCurrentPaletteAsPaletteColor() {
	return _picturePalette;
}

void NativeScummWrapper::NativeScummWrapperPaletteManager::setCurrentPaletteHash(uint32 value) {
	_currentPaletteHash = value;
}

uint32 NativeScummWrapper::NativeScummWrapperPaletteManager::getCurrentCursorPaletteHash() {
	if (!_cursorPaletteDisabled) {
		return _currentCursorPaletteHash;
	}
	else {
		return _currentPaletteHash;
	}

}

void NativeScummWrapper::NativeScummWrapperPaletteManager::setCurrentCursorPaletteHash(uint32 value) {
	_currentCursorPaletteHash = value;
}

uint32 NativeScummWrapper::NativeScummWrapperPaletteManager::populatePicturePalette(const byte *colors, uint start, uint num) {
	return populatePalette(_picturePalette, colors, start, num);
}

uint32 NativeScummWrapper::NativeScummWrapperPaletteManager::populateCursorPalette(const byte *colors, uint start, uint num) {
	return populatePalette(_cursorPalette, colors, start, num);
}

void NativeScummWrapper::NativeScummWrapperPaletteManager::setCursorPaletteDisabled(bool value) {
	_cursorPaletteDisabled = value;
}

void NativeScummWrapper::NativeScummWrapperPaletteManager::grabPalette(byte *colors, uint start, uint num) const {
	memcpy(colors, _pictureColor + start, num);
}

bool NativeScummWrapper::NativeScummWrapperPaletteManager::haveSeenPalette(uint32 paletteHash) {
	return palettesSeen[paletteHash];
}

void NativeScummWrapper::NativeScummWrapperPaletteManager::registerSeenPalette(uint32 paletteHash) {
	palettesSeen[paletteHash] = true;
}

const char* NativeScummWrapper::NativeScummWrapperPaletteManager::getPalette(uint32 paletteHash) {
	return palettes[paletteHash].c_str();
}

const char *NativeScummWrapper::NativeScummWrapperPaletteManager::getPalette() {
	return getPalette(_currentPaletteHash);
}
