#pragma once

#include "common.h";
#include "scummVm.h"
namespace NativeScummWrapper {
	class MouseState {
	public:
		int x;
		int y;
	    int hotX;
	    int hotY;
		int width;
		int height;
		int fullWidth;
		int fullHeight;
		int prevX;
		int prevY;
		int prevW;
		int prevH;
	    int prevHotX;
	    int prevHotY;
		const void *buffer;
		PalletteColor *cursorPallette;
		byte keyColor;

		int adjustedX()
		{
		    return x - hotX;
		}

		int adjustedY()
		{
		    return y - hotY;
		}

		int adjustedPrevX() {
		    return prevX - prevHotX;
	    }

	    int adjustedPrevY() {
		    return prevY - prevHotY;
	    }

		MouseState() {
		    fullHeight = 0;
		    fullWidth = 0;
		    keyColor = 0;
			x = -1;
			y = -1;
		    hotX = hotX;
			hotY = hotY;
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
