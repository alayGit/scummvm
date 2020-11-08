#include "pch.h"



struct PalletteColor;


#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "./ScummVmBrowser/nativeScummWrapper/nativeScummWrapperGraphics.h"
#include "./ScummVmBrowser/nativeScummWrapper/mouseState.h"
#include <stdlib.h>
#include <ctime>
#include <queue>

using namespace NativeScummWrapper;

namespace TestCustomScummVMSubclasses
{
	class MouseTest;
	MouseTest* _mouseTest;

	const int START_X = 0;
	const int START_Y = 0;

	const int START_MOUSE_W = 10;
	const int START_MOUSE_H = 13;
	const int DISPLAY_DEFAULT_WIDTH = 320;
	const int DISPLAY_DEFAULT_HEIGHT = 200;
	const int NO_IN_MOUSE_PALLETTE = 16;

	const byte KEY_COLOR = 0;
	const byte NO_KEY_COLOR = 255;

	static const int NO_IN_PALLETTE = 256;

	class BlotState {
	public:
		int x;
		int y;
		int w;
		int h;
		int noUpdates;
		int callOrder;

		BlotState() {
			callOrder = -1;
		}
	};

	class CopyRectState {
	public:
		const void* buf;
		int pitch;
		int x;
		int y;
		int w;
		int h;
		NativeScummWrapper::PalletteColor* color;
		byte ignore;
		bool isMouseUpdate;
		int noUpdates;
		int callOrder;

		CopyRectState() {
			callOrder = -1;
		}
	};

	static void __stdcall CopyRect(const void* buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor* color, byte ignore, bool isMouseUpdate, int noUpdates);
	static void __stdcall Blot(int x, int y, int w, int h, int noUpdates);
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

		static const int NO_IN_MOUSE_BUFFER = START_MOUSE_W * START_MOUSE_H;
		static const int NO_IN_PIC_BUFFER = DISPLAY_DEFAULT_WIDTH * DISPLAY_DEFAULT_HEIGHT;
		byte mouseBuffer[NO_IN_MOUSE_BUFFER];
		byte pictureBuffer[NO_IN_PIC_BUFFER];

		std::queue<BlotState> _blotStateQueue;

		std::queue<CopyRectState> _copyRectStateQueue;

		NativeScummWrapperGraphics _graphicsManager;
		int _callOrder;
		MouseTest() :_graphicsManager((f_SendScreenBuffers)&CopyRect, (f_Blot)&Blot)
		{
			RandomiseContentsOfPallette(pallette, NO_IN_PALLETTE);
			RandomiseContentsOfPallette(mousePallette, NO_IN_MOUSE_PALLETTE);
			RandomiseContentsOfBuffer(mouseBuffer, NO_IN_MOUSE_BUFFER, NO_IN_MOUSE_PALLETTE);
			RandomiseContentsOfBuffer(pictureBuffer, NO_IN_PIC_BUFFER, NO_IN_PALLETTE);
			_mouseTest = this;
			_graphicsManager.setCursorPalette(mousePallette, 0, NO_IN_MOUSE_PALLETTE);
			_graphicsManager.setPalette(pallette, 0, NO_IN_MOUSE_PALLETTE);
			_callOrder = -1;
			_graphicsManager.setMouseCursor(&mouseBuffer, START_MOUSE_W, START_MOUSE_H, START_X, START_Y, KEY_COLOR);
			_blotStateQueue = std::queue<BlotState>();
			_copyRectStateQueue = std::queue<CopyRectState>();
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

		EXPECT_TRUE(_mouseTest->_blotStateQueue.empty());
		EXPECT_TRUE(_mouseTest->_copyRectStateQueue.empty());
	}

	static void CheckQueuesAreEmpty()
	{
		EXPECT_TRUE(_mouseTest->_blotStateQueue.empty());
		EXPECT_TRUE(_mouseTest->_copyRectStateQueue.empty());
	}
	static void ResetTestState()
	{
		CheckQueuesAreEmpty();
		_mouseTest->_callOrder = -1;
	}

	static void __stdcall CopyRect(const void* buf, int pitch, int x, int y, int w, int h, NativeScummWrapper::PalletteColor* color, byte ignore, bool isMouseUpdate, int noUpdates)
	{
		TestCustomScummVMSubclasses::CopyRectState copyRectState;
		copyRectState.buf = buf;
		copyRectState.pitch = pitch;
		copyRectState.x = x;
		copyRectState.y = y;
		copyRectState.w = w;
		copyRectState.h = h;
		copyRectState.color = color;
		copyRectState.ignore = ignore;
		copyRectState.isMouseUpdate = isMouseUpdate;
		copyRectState.noUpdates = noUpdates;
		copyRectState.callOrder = ++_mouseTest->_callOrder;

		_mouseTest->_copyRectStateQueue.push(copyRectState);
	}


	static void __stdcall Blot(int x, int y, int w, int h, int noUpdates)
	{
		TestCustomScummVMSubclasses::BlotState blotState;
		blotState.h = h;
		blotState.noUpdates = noUpdates;
		blotState.w = w;
		blotState.x = x;
		blotState.y = y;
		blotState.callOrder = ++_mouseTest->_callOrder;

		_mouseTest->_blotStateQueue.push(blotState);
	}

	void MouseStateUpdatedCorrectly(int expectedX, int expectedY, int expectedWidth, int expectedHeight, int previousWidth, int previousHeight, int expectedFullWidth, int expectedFullHeight)
	{
		MouseState mouseState = _mouseTest->_graphicsManager.getMouseState();
		EXPECT_EQ((const void*)&_mouseTest->mouseBuffer, mouseState.buffer);

		for (int i = 0, j = 0; i < NO_IN_MOUSE_PALLETTE; i += 3, j++)
		{
			EXPECT_EQ(_mouseTest->mousePallette[i], (byte)mouseState.cursorPallette[j].r);
			EXPECT_EQ(_mouseTest->mousePallette[i + 1], (byte)mouseState.cursorPallette[j].g);
			EXPECT_EQ(_mouseTest->mousePallette[i + 2], (byte)mouseState.cursorPallette[j].b);
			EXPECT_EQ((byte)255, (byte)mouseState.cursorPallette[j].a);
		}

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

	}

	void AssertBlotCalledWithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight)
	{
		BlotState blotState = _mouseTest->_blotStateQueue.front();
		_mouseTest->_blotStateQueue.pop();

		EXPECT_EQ(expectedX, blotState.x);
		EXPECT_EQ(expectedY, blotState.y);
		EXPECT_EQ(expectedWidth, blotState.w);
		EXPECT_EQ(expectedHeight, blotState.h);
		EXPECT_EQ(2, blotState.noUpdates);
		EXPECT_EQ(0, blotState.callOrder);
	}

	void AssertCopyRectStateCalledwithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight, int expectedPitch, bool isMouseUpdate, void* buffer, byte keyColor, int noUpdates, int callOrder)
	{
		CopyRectState copyRectState = _mouseTest->_copyRectStateQueue.front();
		_mouseTest->_copyRectStateQueue.pop();

		EXPECT_EQ((const void*)buffer, copyRectState.buf);
		EXPECT_EQ(expectedPitch, copyRectState.pitch);
		EXPECT_EQ(expectedX, copyRectState.x);
		EXPECT_EQ(expectedY, copyRectState.y);
		EXPECT_EQ(expectedWidth, copyRectState.w);
		EXPECT_EQ(expectedHeight, copyRectState.h);
		EXPECT_EQ(keyColor, copyRectState.ignore);
		EXPECT_EQ(isMouseUpdate, copyRectState.isMouseUpdate);
		EXPECT_EQ(noUpdates, copyRectState.noUpdates);
		EXPECT_EQ(callOrder, copyRectState.callOrder);
	}

	void AssertCopyRectStateCalledwithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight)
	{
		AssertCopyRectStateCalledwithCorrectPositionAndSize(expectedX, expectedY, expectedWidth, expectedHeight, expectedWidth, true, (void*)&_mouseTest->mouseBuffer, KEY_COLOR, 2, 1);
	}

	void MouseStateUpdatedCorrectly(int expectedX, int expectedY, int expectedWidth, int expectedHeight, int previousWidth, int previousHeight)
	{
		MouseStateUpdatedCorrectly(expectedX, expectedY, expectedWidth, expectedHeight, previousWidth, previousHeight, expectedWidth, expectedHeight);
	}


	TEST_F(MouseTest, CanMoveMouseWithinBounds)
	{
		const int X = 45;
		const int Y = 56;
		_graphicsManager.warpMouse(X, Y);

		AssertBlotCalledWithCorrectPositionAndSize(0, 0, START_MOUSE_W, START_MOUSE_H);
		AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H);
		MouseStateUpdatedCorrectly(X, Y, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		ResetTestState();

		const int X3 = 20;
		const int Y2 = 50;
		_graphicsManager.warpMouse(X3, Y2);

		AssertBlotCalledWithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H);
		AssertCopyRectStateCalledwithCorrectPositionAndSize(X3, Y2, START_MOUSE_W, START_MOUSE_H);
		MouseStateUpdatedCorrectly(X3, Y2, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		CheckQueuesAreEmpty();
	}

	TEST_F(MouseTest, CannotMoveMouseOutOfBoundsXPos)
	{
		MouseState mouseState1 = _graphicsManager.getMouseState();

		const int X = 4005;
		const int Y = 56;
		_graphicsManager.warpMouse(X, Y);

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

		MouseState mouseState2 = _graphicsManager.getMouseState();

		AssertMouseNotMoved(mouseState1, mouseState2);

		CheckQueuesAreEmpty();
	}

	TEST_F(MouseTest, WidthDoesNotShrinkOnXBoundry)
	{
		const int X = 310;
		const int Y = 56;
		_graphicsManager.warpMouse(X, Y);

		AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H);
		MouseStateUpdatedCorrectly(X, Y, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		EXPECT_TRUE(_copyRectStateQueue.empty());
	}

	TEST_F(MouseTest, WidthShrinksOverXBoundry)
	{
		const int X = DISPLAY_DEFAULT_WIDTH - (START_MOUSE_W - 1); //Index starts at 0
		const int Y = 56;
		_graphicsManager.warpMouse(X, Y);

		AssertBlotCalledWithCorrectPositionAndSize(0, 0, START_MOUSE_W, START_MOUSE_H);
		AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W - 2, START_MOUSE_H, START_MOUSE_W, true, &mouseBuffer, KEY_COLOR, 2, 1);
		MouseStateUpdatedCorrectly(X, Y, START_MOUSE_W - 2, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		ResetTestState();

		const int X3 = 20;
		const int Y2 = 50;
		_graphicsManager.warpMouse(X3, Y2);

		AssertBlotCalledWithCorrectPositionAndSize(X, Y, START_MOUSE_W - 2, START_MOUSE_H);
		AssertCopyRectStateCalledwithCorrectPositionAndSize(X3, Y2, START_MOUSE_W, START_MOUSE_H);
		MouseStateUpdatedCorrectly(X3, Y2, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		CheckQueuesAreEmpty();
	}

	TEST_F(MouseTest, WidthShrinksOverYBoundry)
	{
		const int X = 12;
		const int Y = DISPLAY_DEFAULT_HEIGHT - (START_MOUSE_H - 1); //Index starts at 0
		_graphicsManager.warpMouse(X, Y);

		AssertBlotCalledWithCorrectPositionAndSize(0, 0, START_MOUSE_W, START_MOUSE_H);
		AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H - 2, START_MOUSE_W, true, &mouseBuffer, KEY_COLOR, 2, 1);
		MouseStateUpdatedCorrectly(X, Y, START_MOUSE_W, START_MOUSE_H - 2, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		ResetTestState();

		const int X3 = 20;
		const int Y2 = 50;
		_graphicsManager.warpMouse(X3, Y2);

		AssertBlotCalledWithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H - 2);
		AssertCopyRectStateCalledwithCorrectPositionAndSize(X3, Y2, START_MOUSE_W, START_MOUSE_H);
		MouseStateUpdatedCorrectly(X3, Y2, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

		CheckQueuesAreEmpty();
	}

	TEST_F(MouseTest, CanChangeCursor)
	{
		const int NEW_CURSOR_WIDTH = 25;
		const int NEW_CURSOR_HEIGHT = 15;
		const int NEW_CURSOR_X = 15;
		const int NEW_CURSOR_Y = 33;
		const int NEW_CURSOR_BUFFER_LENGTH = NEW_CURSOR_WIDTH * NEW_CURSOR_HEIGHT;

		byte newMouseBuffer[NEW_CURSOR_WIDTH * NEW_CURSOR_HEIGHT];
		RandomiseContentsOfBuffer(newMouseBuffer, NEW_CURSOR_BUFFER_LENGTH, NO_IN_MOUSE_PALLETTE);

		_graphicsManager.setMouseCursor(&newMouseBuffer, NEW_CURSOR_WIDTH, NEW_CURSOR_HEIGHT, NEW_CURSOR_X, NEW_CURSOR_Y, KEY_COLOR);

		AssertBlotCalledWithCorrectPositionAndSize(0, 0, START_MOUSE_W, START_MOUSE_H);

		EXPECT_TRUE(_blotStateQueue.empty());
	}

	TEST_F(MouseTest, CanDrawRectangleOnScreen)
	{
		const int X = 20;
		const int Y = 50;
		_graphicsManager.warpMouse(X, Y);

		AssertBlotCalledWithCorrectPositionAndSize(0, 0, START_MOUSE_W, START_MOUSE_H);
		AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H);

		ResetTestState();

		_graphicsManager.copyRectToScreen(pictureBuffer, DISPLAY_DEFAULT_WIDTH, 0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT);

		AssertCopyRectStateCalledwithCorrectPositionAndSize(0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT, DISPLAY_DEFAULT_WIDTH, false, &pictureBuffer, NO_KEY_COLOR, 2, 0);
		AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H);
	}

	int main(int argc, char* argv[])
	{
		return 0;
	}
}
