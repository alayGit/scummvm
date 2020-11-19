#pragma once
/* ScummVM - Graphic Adventure Engine
 *
 * ScummVM is the legal property of its developers, whose names
 * are too numerous to list here. Please refer to the COPYRIGHT
 * file distributed with this source distribution.
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 *
 */

#include <windows.h>
#include "scummVm.h"
#include "palletteColor.h"
#include "mouseState.h"
#include <functional>
#include <vector>
#include "./ScummVmBrowser/ZLibCompression/ZLibCompression.h"

#include "C:\\scumm\\ScummVmBrowser\\LaunchDebugger\\LaunchDebugger.h"

class NativeScummWrapperEvents;

#if !defined(_WIN32_WCE) && !defined(__SYMBIAN32__)
#define USE_OSD 1
#endif

//ScreenUpdated(int pitch, int x, int y, int w, int h);
/**
 * Base class for a SDL based graphics manager.
 */

#define DISPLAY_DEFAULT_WIDTH 320
#define DISPLAY_DEFAULT_HEIGHT 200
#define DISPLAY_DEFAULT_SIZE 6400
#define NO_BYTES_PER_PIXEL 4

namespace NativeScummWrapper {

	const int WHOLE_SCREEN_BUFFER_LENGTH = DISPLAY_DEFAULT_WIDTH * DISPLAY_DEFAULT_HEIGHT * NO_BYTES_PER_PIXEL;

	struct ScreenBuffer {
	    int x;
	    int y;
	    int w;
	    int h;
	    int length;
	    byte *buffer;
    };

	typedef void(__stdcall *f_SendScreenBuffers)(ScreenBuffer*, int);


class NativeScummWrapperGraphics : virtual public GraphicsManager {
	public:
		NativeScummWrapperGraphics(f_SendScreenBuffers copyRect);
	    ~NativeScummWrapperGraphics();

		virtual void copyRectToScreen(const void *buf, int pitch, int x, int y, int w, int h) override;
		virtual bool hasFeature(OSystem::Feature f) const;
		virtual void setFeatureState(OSystem::Feature f, bool enable);
		virtual bool getFeatureState(OSystem::Feature f) const;
		virtual const OSystem::GraphicsMode *getSupportedGraphicsModes() const override;
		virtual int getDefaultGraphicsMode() const override;
		virtual bool setGraphicsMode(int mode) override;
		virtual void resetGraphicsScale() override;
		virtual int getGraphicsMode() const override;
		//--

		virtual void initSize(uint width, uint height, const Graphics::PixelFormat *format = NULL) override;
		virtual int getScreenChangeID() const override;

		virtual void beginGFXTransaction() override;
		virtual OSystem::TransactionError endGFXTransaction() override;

		virtual int16 getHeight() const override;
		virtual int16 getWidth() const override;
		virtual void setPalette(const byte *colors, uint start, uint num) override;
		virtual void grabPalette(byte *colors, uint start, uint num) const override;
		virtual Graphics::Surface *lockScreen() override;
		virtual void unlockScreen() override;
		virtual void fillScreen(uint32 col) override;
		virtual void updateScreen() override;
	    virtual void setShakePos(int shakeXOffset, int shakeYOffset)override;
		virtual void setFocusRectangle(const Common::Rect &rect) override;
		virtual void clearFocusRectangle() override;

		virtual void showOverlay() override;
		virtual void hideOverlay() override;
		virtual Graphics::PixelFormat getOverlayFormat() const override;
		virtual void clearOverlay() override;
		virtual void grabOverlay(void *buf, int pitch) const override;
		virtual void copyRectToOverlay(const void *buf, int pitch, int x, int y, int w, int h) override;
		virtual int16 getOverlayHeight() const override;
		virtual int16 getOverlayWidth() const override;

		virtual bool showMouse(bool visible) override;
		virtual void warpMouse(int x, int y) override;
		virtual void setMouseCursor(const void *buf, uint w, uint h, int hotspotX, int hotspotY, uint32 keycolor, bool dontScale = false, const Graphics::PixelFormat *format = NULL) override;
		virtual void setCursorPalette(const byte *colors, uint start, uint num) override;
		virtual Graphics::PixelFormat getScreenFormat() const;
		virtual Common::List<Graphics::PixelFormat> getSupportedFormats() const;
		bool screenUpdateOverlapsMouse(int x, int y, int w, int h);
		bool positionInRange(int x, int y);
		MouseState getMouseState();
	    byte* GetWholeScreenBufferCompressed(int &width, int &height, int &bufferSize);
	    byte* GetWholeScreenBufferRaw(int &width, int &height, int &bufferSize);

	private:
	    std::vector<ScreenBuffer> _drawingCommands;
		f_SendScreenBuffers _copyRect;
		PalletteColor *_picturePalette;
		PalletteColor *_cursorPalette;
		PalletteColor *allocatePallette();
		MouseState _cliMouse;
	    bool _screenInited = false;
		void populatePalette(PalletteColor *pallette, const byte *colors, uint start, uint num);
		int restrictWidthToScreenBounds(int x, int width);
		int restrictHeightToScreenBounds(int y, int height);
		void setCurrentMouseStateToPrevious();
	    byte* GetBlottedBuffer(int x, int y, int w, int h);
	    byte* ScreenUpdated(const void *buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor *color, byte ignore, bool isMouseUpdate);
	    void UpdatePictureBuffer(byte *pictureArray, const void *buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor *color, byte ignore);
	    void UpdateWholeScreenBuffer(byte *pictureArray, byte *wholeScreenBuffer, int x, int y, int w, int h);
	    ScreenBuffer GetScreenBuffer(const void *buf, int x, int y, int w, int h);
	    void InitScreen();
	    byte *_wholeScreenBuffer;
	    byte *_wholeScreenBufferNoMouse;
	    HANDLE _wholeScreenMutex;
	};
} // namespace NativeScummWrapper
