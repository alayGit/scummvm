struct PalletteColor;


#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "./ScummVmBrowser/nativeScummWrapper/nativeScummWrapperGraphics.h"
#include "./ScummVmBrowser/nativeScummWrapper/mouseState.h"
#include "gtest/gtest.h"
#include <stdlib.h>
#include <ctime>
#include <queue>

using namespace NativeScummWrapper;

namespace TestCustomScummVMSubclasses
{
	class MouseTest;
	MouseTest* _mouseTest;

	const int START_X = 50;
	const int START_Y = 50;
    const int START_HOTX = 10;
    const int START_HOTY = 12;

	const int START_MOUSE_W = 10;
	const int START_MOUSE_H = 13;
	const int NO_IN_MOUSE_PALLETTE = 16;

	const byte KEY_COLOR = 0;
	const byte NO_KEY_COLOR = 255;

	static const int NO_IN_PALLETTE = 256;

	class TestNativeScummWrapperGraphics: public NativeScummWrapper::NativeScummWrapperGraphics {
	public:

		TestNativeScummWrapperGraphics(f_SendScreenBuffers copyRect, NativeScummWrapper::NativeScummWrapperPaletteManager* _paletteManager) : NativeScummWrapperGraphics(copyRect, _paletteManager) {

		}

		void triggerUpdateScreen() {
		    updateScreen();
		}
	};

	class CopyRectState {
	public:
		const void* buf;
		int x;
		int y;
		int w;
		int h;
		int callOrder;

		CopyRectState() {
		    buf = nullptr;
		    x = 0;
		    y = 0;
		    w = 0;
		    h = 0;
			callOrder = -1;
		}
	};

	static void __stdcall CopyRect(ScreenBuffer* screenBuffer, int length);
	static void ResetTestState();
	static void CheckQueuesAreEmpty();

	static void RandomiseContentsOfPallette(byte* buffer, int length)
	{
		for (int i = 0; i < length; i++)
		{
			buffer[i] = rand() % 256 - 1;
		}
	}

	static void RandomiseContentsOfBuffer(byte* buffer, int length, int palletteLength)
	{
		for (int i = 0; i < length; i++)
		{
			int x = rand() % palletteLength - 1;
			buffer[i] = x;
		}
	}


	class MouseTest : public::testing::Test
	{
	public:
		byte pallette[NO_IN_PALLETTE];

		byte mousePallette[NO_IN_MOUSE_PALLETTE];
	    NativeScummWrapper::NativeScummWrapperPaletteManager *_paletteManager;

		static const int NO_IN_MOUSE_BUFFER = START_MOUSE_W * START_MOUSE_H;
		static const int NO_IN_PIC_BUFFER = DISPLAY_DEFAULT_WIDTH * DISPLAY_DEFAULT_HEIGHT;
		byte mouseBuffer[NO_IN_MOUSE_BUFFER];
		byte pictureBuffer[NO_IN_PIC_BUFFER];

		std::queue<CopyRectState> _copyRectStateQueue;

		TestNativeScummWrapperGraphics _graphicsManager;
		int _callOrder;
		MouseTest() :_graphicsManager((f_SendScreenBuffers)&CopyRect, _paletteManager = new NativeScummWrapper::NativeScummWrapperPaletteManager())
		{
			RandomiseContentsOfPallette(pallette, NO_IN_PALLETTE);
			RandomiseContentsOfPallette(mousePallette, NO_IN_MOUSE_PALLETTE);
			RandomiseContentsOfBuffer(mouseBuffer, NO_IN_MOUSE_BUFFER, NO_IN_MOUSE_PALLETTE);
			RandomiseContentsOfBuffer(pictureBuffer, NO_IN_PIC_BUFFER, NO_IN_PALLETTE);
			_mouseTest = this;
			_graphicsManager.setCursorPalette(mousePallette, 0, NO_IN_MOUSE_PALLETTE);
			_graphicsManager.setPalette(pallette, 0, NO_IN_MOUSE_PALLETTE);
			_callOrder = -1;
		    _graphicsManager.setMouseCursor(&mouseBuffer, START_MOUSE_W, START_MOUSE_H, START_HOTX, START_HOTY, KEY_COLOR);
		    _graphicsManager.warpMouse(START_X, START_Y);
			_copyRectStateQueue = std::queue<CopyRectState>();

			int buffer[1];
		    buffer[0] = 0;
		    _graphicsManager.copyRectToScreen(&buffer, DISPLAY_DEFAULT_WIDTH, 0, 0, 0, 0); //This ensures the thing is inited before we start
		    _graphicsManager.triggerUpdateScreen(); //Clear away updates we are not interested in
			_copyRectStateQueue = std::queue<CopyRectState>(); //Clear the queue
		    ResetTestState();
		}
	};


	void AssertMouseNotMoved(MouseState mouseBefore, MouseState mouseAfter)
	{
		EXPECT_EQ(mouseBefore.fullHeight, mouseAfter.fullHeight);
		EXPECT_EQ(mouseBefore.fullWidth, mouseAfter.fullWidth);
		EXPECT_EQ(mouseBefore.height, mouseAfter.height);
		EXPECT_EQ(mouseBefore.keyColor, mouseAfter.keyColor);
		EXPECT_EQ(mouseBefore.prevH, mouseAfter.prevH);
		EXPECT_EQ(mouseBefore.prevW, mouseAfter.prevW);
		EXPECT_EQ(mouseBefore.prevX, mouseAfter.prevX);
		EXPECT_EQ(mouseBefore.prevY, mouseAfter.prevY);
		EXPECT_EQ(mouseBefore.width, mouseAfter.width);
		EXPECT_EQ(mouseBefore.x, mouseAfter.y);

		EXPECT_TRUE(_mouseTest->_copyRectStateQueue.empty());
	}

	static void CheckQueuesAreEmpty()
	{
		EXPECT_TRUE(_mouseTest->_copyRectStateQueue.empty());
	}
	static void ResetTestState()
	{
		CheckQueuesAreEmpty();
		_mouseTest->_callOrder = -1;
	}

	static void __stdcall CopyRect(ScreenBuffer *screenBuffer, int length)
	{
		for (int i = 0; i < length; i++) {
		    TestCustomScummVMSubclasses::CopyRectState copyRectState;
		    copyRectState.buf = screenBuffer[i].buffer;
		    copyRectState.x = screenBuffer[i].x;
		    copyRectState.y = screenBuffer[i].y;
		    copyRectState.w = screenBuffer[i].w;
		    copyRectState.h = screenBuffer[i].h;
		    copyRectState.callOrder = ++_mouseTest->_callOrder;
		    _mouseTest->_copyRectStateQueue.push(copyRectState);
	    }
	}

	void MouseStateUpdatedCorrectly(int expectedX, int expectedY, int expectedHotX, int expectedHotY, int expectedPrevHotX, int expectedPrevHotY, int expectedWidth, int expectedHeight, int previousWidth, int previousHeight, int expectedFullWidth, int expectedFullHeight)
	{
		MouseState mouseState = _mouseTest->_graphicsManager.getMouseState();
		EXPECT_EQ((const void*)&_mouseTest->mouseBuffer, mouseState.buffer);

		EXPECT_EQ(expectedFullWidth, mouseState.fullWidth);
		EXPECT_EQ(expectedFullHeight, mouseState.fullHeight);
		EXPECT_EQ(expectedHeight, mouseState.height);
		EXPECT_EQ((byte)0, mouseState.keyColor);
		EXPECT_EQ(expectedHeight, mouseState.prevH);
		EXPECT_EQ(expectedWidth, mouseState.prevW);
		EXPECT_EQ(expectedX, mouseState.prevX);
		EXPECT_EQ(expectedY, mouseState.prevY);
		EXPECT_EQ(expectedWidth, mouseState.width);
		EXPECT_EQ(expectedX, mouseState.x);
		EXPECT_EQ(expectedY, mouseState.y);
	    EXPECT_EQ(expectedHotX, mouseState.hotX);
	    EXPECT_EQ(expectedHotY, mouseState.hotY);
	    EXPECT_EQ(expectedPrevHotX, mouseState.prevHotX);
	    EXPECT_EQ(expectedPrevHotY, mouseState.prevHotY);
	}

	void AssertCopyRectCalledWithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight)
	{
	    CopyRectState copyRectState = _mouseTest->_copyRectStateQueue.front();
		_mouseTest->_copyRectStateQueue.pop();

		EXPECT_EQ(expectedX, copyRectState.x);
		EXPECT_EQ(expectedY, copyRectState.y);
		EXPECT_EQ(expectedWidth, copyRectState.w);
		EXPECT_EQ(expectedHeight, copyRectState.h);
	}

	void AssertCopyRectStateCalledwithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight, int expectedPitch, void *buffer, int callOrder) {
	    AssertCopyRectCalledWithCorrectPositionAndSize(expectedX, expectedY, expectedWidth, expectedHeight);
	}

	void AssertCopyRectStateCalledwithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight, int expectedPitch, void* buffer, int callOrder, bool checkForBufferAddressEquality)
	{
		CopyRectState copyRectState = _mouseTest->_copyRectStateQueue.front();
		_mouseTest->_copyRectStateQueue.pop();

		EXPECT_TRUE((const void*)buffer == copyRectState.buf || !checkForBufferAddressEquality);
		EXPECT_EQ(expectedX, copyRectState.x);
		EXPECT_EQ(expectedY, copyRectState.y);
		EXPECT_EQ(expectedWidth, copyRectState.w);
		EXPECT_EQ(expectedHeight, copyRectState.h);
		EXPECT_EQ(callOrder, copyRectState.callOrder);
	}

	void AssertCopyRectStateCalledwithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight, int callOrder, bool checkForBufferAddressEquality) {
	    AssertCopyRectStateCalledwithCorrectPositionAndSize(expectedX, expectedY, expectedWidth, expectedHeight, expectedWidth, (void *)&_mouseTest->mouseBuffer, callOrder, checkForBufferAddressEquality);
    }

	void AssertCopyRectStateCalledwithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight, bool checkForBufferAddressEquality) {
	    AssertCopyRectStateCalledwithCorrectPositionAndSize(expectedX, expectedY, expectedWidth, expectedHeight, 0, checkForBufferAddressEquality);
    }

	void AssertCopyRectStateCalledwithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight)
	{
		AssertCopyRectStateCalledwithCorrectPositionAndSize(expectedX, expectedY, expectedWidth, expectedHeight, expectedWidth, (void*)&_mouseTest->mouseBuffer, 1, false);
	}


	void MouseStateUpdatedCorrectly(int expectedX, int expectedY, int expectedHotX, int expectedHotY, int expectedPrevHotX, int expectedPrevHotY, int expectedWidth, int expectedHeight, int previousWidth, int previousHeight)
	{
		MouseStateUpdatedCorrectly(expectedX, expectedY, expectedHotX, expectedHotY, expectedPrevHotX, expectedPrevHotY, expectedWidth, expectedHeight, previousWidth, previousHeight, expectedWidth, expectedHeight);
	}

	TEST_F(MouseTest, CanMoveMouseWithinBounds)
	{
		const int X = 45;
		const int Y = 56;
		_graphicsManager.warpMouse(X, Y);
	    _graphicsManager.triggerUpdateScreen();

	    AssertCopyRectStateCalledwithCorrectPositionAndSize(X - START_HOTX, Y - START_HOTY, START_MOUSE_W, START_MOUSE_H, false);
		MouseStateUpdatedCorrectly(X, Y,START_HOTX, START_HOTY,START_HOTX,START_HOTY,START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		ResetTestState();

		const int X2 = 20;
		const int Y2 = 50;
		_graphicsManager.warpMouse(X2, Y2);
	    _graphicsManager.triggerUpdateScreen();

		AssertCopyRectCalledWithCorrectPositionAndSize(X - START_HOTX, Y - START_HOTY, START_MOUSE_W, START_MOUSE_H);
	    AssertCopyRectStateCalledwithCorrectPositionAndSize(X2 - START_HOTX, Y2 - START_HOTY, START_MOUSE_W, START_MOUSE_H, 1, false);
		MouseStateUpdatedCorrectly(X2, Y2, START_HOTX, START_HOTY, START_HOTX, START_HOTY, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		CheckQueuesAreEmpty();
	}

	TEST_F(MouseTest, CannotMoveMouseOutOfBoundsXPos)
	{
		MouseState mouseState1 = _graphicsManager.getMouseState();

		const int X = 4005;
		const int Y = 56;
		_graphicsManager.warpMouse(X, Y);
	    _graphicsManager.triggerUpdateScreen();

		MouseState mouseState2 = _graphicsManager.getMouseState();

		AssertMouseNotMoved(mouseState1, mouseState2);

		CheckQueuesAreEmpty();
	}

	TEST_F(MouseTest, CannotMoveMouseOutOfBoundsXNeg)
	{
		MouseState mouseState1 = _graphicsManager.getMouseState();

		const int X = -3;
		const int Y = 56;
		_graphicsManager.warpMouse(X, Y);

		MouseState mouseState2 = _graphicsManager.getMouseState();

		AssertMouseNotMoved(mouseState1, mouseState2);

		CheckQueuesAreEmpty();
	}


	TEST_F(MouseTest, CannotMoveMouseOutOfBoundsYPos)
	{
		MouseState mouseState1 = _graphicsManager.getMouseState();

		const int X = 40;
		const int Y = 56666;
		_graphicsManager.warpMouse(X, Y);
	    _graphicsManager.triggerUpdateScreen();

		MouseState mouseState2 = _graphicsManager.getMouseState();

		AssertMouseNotMoved(mouseState1, mouseState2);

		CheckQueuesAreEmpty();
	}

	TEST_F(MouseTest, CannotMoveMouseOutOfBoundsYNeg)
	{
		MouseState mouseState1 = _graphicsManager.getMouseState();

		const int X = 30;
		const int Y = -56;
		_graphicsManager.warpMouse(X, Y);
	    _graphicsManager.triggerUpdateScreen();

		MouseState mouseState2 = _graphicsManager.getMouseState();

		AssertMouseNotMoved(mouseState1, mouseState2);

		CheckQueuesAreEmpty();
	}

	TEST_F(MouseTest, WidthDoesNotShrinkOnXBoundry)
	{
	    const int X = (DISPLAY_DEFAULT_WIDTH + START_HOTX) - START_MOUSE_W;
		const int Y = 56;
		_graphicsManager.warpMouse(X, Y);
	    _graphicsManager.triggerUpdateScreen();

	    MouseStateUpdatedCorrectly(X, Y, START_HOTX, START_HOTY, START_HOTX, START_HOTY, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);
	}

	TEST_F(MouseTest, WidthDoesNotShrinkOnYBoundry) {
	    const int X = 12;
	    const int Y = (DISPLAY_DEFAULT_HEIGHT + START_HOTY) - START_MOUSE_H;

	    _graphicsManager.warpMouse(X, Y);
	    _graphicsManager.triggerUpdateScreen();

	    MouseStateUpdatedCorrectly(X, Y, START_HOTX, START_HOTY, START_HOTX, START_HOTY, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);
    }

	TEST_F(MouseTest, WidthShrinksOverXBoundry)
	{
		const int X = (DISPLAY_DEFAULT_WIDTH + START_HOTX) - (START_MOUSE_W - 1); //Index starts at 0
		const int Y = 56;
		_graphicsManager.warpMouse(X, Y);
	    _graphicsManager.triggerUpdateScreen();
	    MouseStateUpdatedCorrectly(X, Y, START_HOTX, START_HOTY, START_HOTX, START_HOTY, START_MOUSE_W - 2, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		const int X3 = 20;
		const int Y2 = 50;
		_graphicsManager.warpMouse(X3, Y2);
	    _graphicsManager.triggerUpdateScreen();

	    MouseStateUpdatedCorrectly(X3, Y2, START_HOTX, START_HOTY, START_HOTX, START_HOTY, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);
	}

	TEST_F(MouseTest, WidthShrinksOverYBoundry)
	{
		const int X = 12;
		const int Y = (DISPLAY_DEFAULT_HEIGHT + START_HOTY) - (START_MOUSE_H - 1); //Index starts at 0
		_graphicsManager.warpMouse(X, Y);
	    _graphicsManager.triggerUpdateScreen();

	    MouseStateUpdatedCorrectly(X, Y, START_HOTX, START_HOTY, START_HOTX, START_HOTY, START_MOUSE_W, START_MOUSE_H - 2, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		const int X2 = 20;
		const int Y2 = 50;
		_graphicsManager.warpMouse(X2, Y2);
	    _graphicsManager.triggerUpdateScreen();

	    MouseStateUpdatedCorrectly(X2, Y2, START_HOTX, START_HOTY, START_HOTX, START_HOTY, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);
	}

	TEST_F(MouseTest, CanChangeCursor) {
	    const int NEW_CURSOR_WIDTH = 25;
	    const int NEW_CURSOR_HEIGHT = 15;
	    const int NEW_CURSOR_X = 15;
	    const int NEW_CURSOR_Y = 33;
	    const int NEW_CURSOR_HOTX = 6;
	    const int NEW_CURSOR_HOTY = 6;
	    const int NEW_CURSOR_BUFFER_LENGTH = NEW_CURSOR_WIDTH * NEW_CURSOR_HEIGHT;
		
	    byte newMouseBuffer[NEW_CURSOR_WIDTH * NEW_CURSOR_HEIGHT];
	    RandomiseContentsOfBuffer(newMouseBuffer, NEW_CURSOR_BUFFER_LENGTH, NO_IN_MOUSE_PALLETTE);

		_graphicsManager.warpMouse(NEW_CURSOR_X, NEW_CURSOR_Y);
	    _graphicsManager.setMouseCursor(&newMouseBuffer, NEW_CURSOR_WIDTH, NEW_CURSOR_HEIGHT, NEW_CURSOR_HOTX, NEW_CURSOR_HOTY, KEY_COLOR);
		_graphicsManager.triggerUpdateScreen();
		
		AssertCopyRectStateCalledwithCorrectPositionAndSize(NEW_CURSOR_X - START_HOTX, NEW_CURSOR_Y - START_HOTY, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, newMouseBuffer, 1);
	    AssertCopyRectCalledWithCorrectPositionAndSize(NEW_CURSOR_X - START_HOTX, NEW_CURSOR_Y - START_HOTY, START_MOUSE_W, START_MOUSE_H);
	    AssertCopyRectStateCalledwithCorrectPositionAndSize(NEW_CURSOR_X - NEW_CURSOR_HOTX, NEW_CURSOR_Y - NEW_CURSOR_HOTY, NEW_CURSOR_WIDTH, NEW_CURSOR_HEIGHT, NEW_CURSOR_WIDTH, newMouseBuffer, 1);
	   

	    EXPECT_TRUE(_copyRectStateQueue.empty());
    }

	TEST_F(MouseTest, CanDrawRectangleOnScreen)
	{
		const int X = 20;
		const int Y = 50;
		_graphicsManager.warpMouse(X, Y);
	    _graphicsManager.triggerUpdateScreen();

		AssertCopyRectStateCalledwithCorrectPositionAndSize(X - START_HOTX, Y - START_HOTY, START_MOUSE_W, START_MOUSE_H, false);

		ResetTestState();

		_graphicsManager.copyRectToScreen(pictureBuffer, DISPLAY_DEFAULT_WIDTH, 0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT);
	    _graphicsManager.triggerUpdateScreen();

		AssertCopyRectStateCalledwithCorrectPositionAndSize(0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT, DISPLAY_DEFAULT_WIDTH, &pictureBuffer, false);
		AssertCopyRectStateCalledwithCorrectPositionAndSize(X - START_HOTX, Y - START_HOTY, START_MOUSE_W, START_MOUSE_H, 1, false);
	}

	int main(int argc, char* argv[])
	{
		return 0;
	}
}
