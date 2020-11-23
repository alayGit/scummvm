#pragma once

#include <unordered_map>
#include <string>

#define DISPLAY_DEFAULT_WIDTH 320
#define DISPLAY_DEFAULT_HEIGHT 200
#define DISPLAY_DEFAULT_SIZE 6400
#define NO_BYTES_PER_PIXEL 4
#define NO_COLOURS 256
#define NO_DIGITS_IN_PALETTE_VALUE 3

namespace NativeScummWrapper {
typedef struct PalletteColor {
	uint r;
	uint g;
	uint b;
	uint a;
} Color;
typedef signed __int8 Uint8;

enum DrawingCommand { DrawScreen,
	                  DrawMouse };

struct ScreenBuffer {
	int x;
	int y;
	int w;
	int h;
	int length;
	int pitch;
	byte *buffer;
	uint32 paletteHash;
	byte* compressedPalette;
	int compressedPalletteLength;
};

inline std::string PadPaletteValue(uint value) {
	std::string valueAsString = std::to_string(value);

	while (valueAsString.length() < 3) {
		valueAsString = "0" + valueAsString;
	}

	return valueAsString;
}

inline std::string GetPaletteAsString(PalletteColor color) {
	return PadPaletteValue(color.a) + PadPaletteValue(color.r) + PadPaletteValue(color.g) + PadPaletteValue(color.b);
}

inline std::string GetPalettesAsString(PalletteColor* pallette, int length)
{
	std::string toHash = "";

	for (int i = 0; i < length; i++)
	{
		toHash = toHash + GetPaletteAsString(pallette[i]);
	}

	return toHash;
}

} // namespace NativeScummWrapper
