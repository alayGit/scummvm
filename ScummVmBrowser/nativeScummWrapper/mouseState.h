#pragma once

#include "palletteColor.h";
#include "scummVm.h"
namespace NativeScummWrapper {
	class MouseState {
	public:
		int x;
		int y;
		int width;
		int height;
		int fullWidth;
		int fullHeight;
		int prevX;
		int prevY;
		int prevW;
		int prevH;
		const void *buffer;
		PalletteColor *cursorPallette;
		byte keyColor;

		MouseState() {
		    fullHeight = 0;
		    fullWidth = 0;
		    keyColor = 0;
			x = -1;
			y = -1;
			width = -1;
			height = -1;
			buffer = nullptr;
			cursorPallette = nullptr;
			prevX = -1;
			prevY = -1;
			prevW = -1;
			prevH = -1;
		}
	};
} // namespace NativeScummWrapper
