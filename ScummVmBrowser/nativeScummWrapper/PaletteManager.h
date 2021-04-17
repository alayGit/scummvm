#pragma once
#include "scummVm.h"
#include "common.h"
namespace NativeScummWrapper {
class nativeScummWrapperPaletteManager {
	PalletteColor *_picturePalette;
	PalletteColor *_cursorPalette;
	byte *_pictureColor;
	std::unordered_map<int, std::string> palettes;
	std::unordered_map<int, bool> palettesSeen;
	uint32 _currentPaletteHash;
	uint32 _currentCursorPaletteHash;
	bool _cursorPaletteDisabled; //If the cursor palette is disabled the picturePalette is used for the mouse as well
	NativeScummWrapper::PalletteColor *allocatePallette();
	uint32 populatePalette(NativeScummWrapper::PalletteColor *pallette, const byte *colors, uint start, uint num);

public:
	nativeScummWrapperPaletteManager();
	~nativeScummWrapperPaletteManager();
	uint32 RememberPalette(NativeScummWrapper::PalletteColor *palette, int length);
	uint32 getCurrentPaletteHash();
	void setCurrentPaletteHash(uint32 value);
	uint32 getCurrentCursorPaletteHash();
	void setCurrentCursorPaletteHash(uint32 value);
	uint32 populatePicturePalette(const byte *colors, uint start, uint num);
	uint32 populateCursorPalette(const byte *colors, uint start, uint num);
	void setCursorPaletteDisabled(bool value);
	void grabPalette(byte *colors, uint start, uint num) const;
	bool haveSeenPalette(uint32 paletteHash);
	void registerSeenPalette(uint32 paletteHash);
	const char* getPalette(uint32 paletteHash);
};
} // namespace NativeScummWrapper
