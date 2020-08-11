
#include "pch.h"
#include "CppUnitTest.h"


struct PalletteColor;


#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "../nativeScummWrapper/nativeScummWrapperGraphics.h"
#include "../nativeScummWrapper/mouseState.h"
#include <stdlib.h>
#include <ctime>
#include <queue>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace NativeScummWrapper;

namespace TestCustomScummVMSubclasses
{

	enum Test {

	};

	TEST_CLASS(TestCustomScummVMSubclasses)
	{

	private:


		static const int START_X = 0;
		static const int START_Y = 0;

		static const int START_MOUSE_W = 10;
		static const int START_MOUSE_H = 13;
		static const int DISPLAY_DEFAULT_WIDTH = 320;
		static const int DISPLAY_DEFAULT_HEIGHT = 200;

		static const byte KEY_COLOR = 0;
		static const byte NO_KEY_COLOR = 255;

		static const int NO_IN_PALLETTE = 256;
		static byte pallette[NO_IN_PALLETTE];

		static const int NO_IN_MOUSE_PALLETTE = 16;
		static byte mousePallette[NO_IN_MOUSE_PALLETTE];

		static const int NO_IN_MOUSE_BUFFER = START_MOUSE_W * START_MOUSE_H;
		static const int NO_IN_PIC_BUFFER = DISPLAY_DEFAULT_WIDTH * DISPLAY_DEFAULT_HEIGHT;
		static byte mouseBuffer[NO_IN_MOUSE_BUFFER];
		static byte pictureBuffer[NO_IN_PIC_BUFFER];

		static int _callOrder;

	public:
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

		static std::queue<BlotState> _blotStateQueue;

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

		static std::queue<CopyRectState> _copyRectStateQueue;



		NativeScummWrapperGraphics _graphicsManager;

		TEST_CLASS_INITIALIZE(ClassInit)
		{
			RandomiseContentsOfPallette(pallette, NO_IN_PALLETTE);
			RandomiseContentsOfPallette(mousePallette, NO_IN_MOUSE_PALLETTE);
			RandomiseContentsOfBuffer(mouseBuffer, NO_IN_MOUSE_BUFFER, NO_IN_MOUSE_PALLETTE);
			RandomiseContentsOfBuffer(pictureBuffer, NO_IN_PIC_BUFFER, NO_IN_PALLETTE);
			ResetTestState();
		}

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

		TEST_METHOD_INITIALIZE(Init)
		{
			TestCustomScummVMSubclasses::_callOrder = -1;
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
			copyRectState.callOrder = ++TestCustomScummVMSubclasses::_callOrder;

			_copyRectStateQueue.push(copyRectState);
		}

		static void __stdcall Blot(int x, int y, int w, int h, int noUpdates)
		{
			TestCustomScummVMSubclasses::BlotState blotState;
			blotState.h = h;
			blotState.noUpdates = noUpdates;
			blotState.w = w;
			blotState.x = x;
			blotState.y = y;
			blotState.callOrder = ++TestCustomScummVMSubclasses::_callOrder;

			_blotStateQueue.push(blotState);
		}

		TestCustomScummVMSubclasses() :_graphicsManager((f_CopyRect)&CopyRect, (f_Blot)&Blot) {
			_graphicsManager.setCursorPalette(mousePallette, 0, NO_IN_MOUSE_PALLETTE);
			_graphicsManager.setPalette(pallette, 0, NO_IN_MOUSE_PALLETTE);
			_graphicsManager.setMouseCursor(&mouseBuffer, START_MOUSE_W, START_MOUSE_H, START_X, START_Y, KEY_COLOR);
			_blotStateQueue = std::queue<BlotState>();
			_copyRectStateQueue = std::queue<CopyRectState>();
		}
		TEST_METHOD(CanMoveMouseWithinBounds)
		{
			const int X = 45;
			const int Y = 56;
			_graphicsManager.warpMouse(X, Y);

			AssertBlotCalledWithCorrectPositionAndSize(0, 0, START_MOUSE_W, START_MOUSE_H);
			AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H);
			MouseStateUpdatedCorrectly(X, Y, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

			ResetTestState();

			const int X2 = 20;
			const int Y2 = 50;
			_graphicsManager.warpMouse(X2, Y2);

			AssertBlotCalledWithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H);
			AssertCopyRectStateCalledwithCorrectPositionAndSize(X2, Y2, START_MOUSE_W, START_MOUSE_H);
			MouseStateUpdatedCorrectly(X2, Y2, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

			CheckQueuesAreEmpty();
		}

		TEST_METHOD(CannotMoveMouseOutOfBoundsXPos)
		{
			MouseState mouseState1 = _graphicsManager.getMouseState();

			const int X = 4005;
			const int Y = 56;
			_graphicsManager.warpMouse(X, Y);

			MouseState mouseState2 = _graphicsManager.getMouseState();

			AssertMouseNotMoved(mouseState1, mouseState2);

			CheckQueuesAreEmpty();
		}

		TEST_METHOD(CannotMoveMouseOutOfBoundsXNeg)
		{
			MouseState mouseState1 = _graphicsManager.getMouseState();

			const int X = -3;
			const int Y = 56;
			_graphicsManager.warpMouse(X, Y);

			MouseState mouseState2 = _graphicsManager.getMouseState();

			AssertMouseNotMoved(mouseState1, mouseState2);

			CheckQueuesAreEmpty();
		}


		TEST_METHOD(CannotMoveMouseOutOfBoundsYPos)
		{
			MouseState mouseState1 = _graphicsManager.getMouseState();

			const int X = 40;
			const int Y = 56666;
			_graphicsManager.warpMouse(X, Y);

			MouseState mouseState2 = _graphicsManager.getMouseState();

			AssertMouseNotMoved(mouseState1, mouseState2);

			CheckQueuesAreEmpty();
		}

		TEST_METHOD(CannotMoveMouseOutOfBoundsYNeg)
		{
			MouseState mouseState1 = _graphicsManager.getMouseState();

			const int X = 30;
			const int Y = -56;
			_graphicsManager.warpMouse(X, Y);

			MouseState mouseState2 = _graphicsManager.getMouseState();

			AssertMouseNotMoved(mouseState1, mouseState2);

			CheckQueuesAreEmpty();
		}

		TEST_METHOD(WidthDoesNotShrinkOnXBoundry)
		{
			const int X = 310;
			const int Y = 56;
			_graphicsManager.warpMouse(X, Y);

			AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H);
			MouseStateUpdatedCorrectly(X, Y, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

			Assert::IsTrue(_copyRectStateQueue.empty());
		}

		TEST_METHOD(WidthShrinksOverXBoundry)
		{
			const int X = DISPLAY_DEFAULT_WIDTH - (START_MOUSE_W - 1); //Index starts at 0
			const int Y = 56;
			_graphicsManager.warpMouse(X, Y);

			AssertBlotCalledWithCorrectPositionAndSize(0, 0, START_MOUSE_W, START_MOUSE_H);
			AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W - 2, START_MOUSE_H, START_MOUSE_W, true, &mouseBuffer, KEY_COLOR, 2, 1);
			MouseStateUpdatedCorrectly(X, Y, START_MOUSE_W - 2, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

			ResetTestState();

			const int X2 = 20;
			const int Y2 = 50;
			_graphicsManager.warpMouse(X2, Y2);

			AssertBlotCalledWithCorrectPositionAndSize(X, Y, START_MOUSE_W - 2, START_MOUSE_H);
			AssertCopyRectStateCalledwithCorrectPositionAndSize(X2, Y2, START_MOUSE_W, START_MOUSE_H);
			MouseStateUpdatedCorrectly(X2, Y2, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

			CheckQueuesAreEmpty();
		}

		TEST_METHOD(WidthShrinksOverYBoundry)
		{
			const int X = 12;
			const int Y = DISPLAY_DEFAULT_HEIGHT - (START_MOUSE_H - 1); //Index starts at 0
			_graphicsManager.warpMouse(X, Y);

			AssertBlotCalledWithCorrectPositionAndSize(0, 0, START_MOUSE_W, START_MOUSE_H);
			AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H - 2, START_MOUSE_W, true, &mouseBuffer, KEY_COLOR, 2, 1);
			MouseStateUpdatedCorrectly(X, Y, START_MOUSE_W, START_MOUSE_H - 2, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

			ResetTestState();

			const int X2 = 20;
			const int Y2 = 50;
			_graphicsManager.warpMouse(X2, Y2);

			AssertBlotCalledWithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H - 2);
			AssertCopyRectStateCalledwithCorrectPositionAndSize(X2, Y2, START_MOUSE_W, START_MOUSE_H);
			MouseStateUpdatedCorrectly(X2, Y2, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H, START_MOUSE_W, START_MOUSE_H);

			CheckQueuesAreEmpty();
		}

		TEST_METHOD(CanChangeCursor)
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

			Assert::IsTrue(_blotStateQueue.empty());
		}

		TEST_METHOD(CanDrawRectangleOnScreen)
		{
			const int X = 20;
			const int Y = 50;
			_graphicsManager.warpMouse(X, Y);

			AssertBlotCalledWithCorrectPositionAndSize(0, 0, START_MOUSE_W, START_MOUSE_H);
			AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H);

			ResetTestState();

			_graphicsManager.copyRectToScreen(pictureBuffer, DISPLAY_DEFAULT_WIDTH, 0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT);
			
			AssertCopyRectStateCalledwithCorrectPositionAndSize(0, 0, DISPLAY_DEFAULT_WIDTH, DISPLAY_DEFAULT_HEIGHT, DISPLAY_DEFAULT_WIDTH, false, &pictureBuffer, NO_KEY_COLOR, 2 , 0);
			AssertCopyRectStateCalledwithCorrectPositionAndSize(X, Y, START_MOUSE_W, START_MOUSE_H);
		}

		void AssertBlotCalledWithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight)
		{
			BlotState blotState = _blotStateQueue.front();
			_blotStateQueue.pop();

			Assert::AreEqual(expectedX, blotState.x);
			Assert::AreEqual(expectedY, blotState.y);
			Assert::AreEqual(expectedWidth, blotState.w);
			Assert::AreEqual(expectedHeight, blotState.h);
			Assert::AreEqual(2, blotState.noUpdates);
			Assert::AreEqual(0, blotState.callOrder);
		}

		void AssertCopyRectStateCalledwithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight)
		{
			AssertCopyRectStateCalledwithCorrectPositionAndSize(expectedX, expectedY, expectedWidth, expectedHeight, expectedWidth, true, (void*)&mouseBuffer, KEY_COLOR, 2, 1);
		}

		void AssertCopyRectStateCalledwithCorrectPositionAndSize(int expectedX, int expectedY, int expectedWidth, int expectedHeight, int expectedPitch, bool isMouseUpdate, void* buffer, byte keyColor, int noUpdates, int callOrder)
		{
			CopyRectState copyRectState = _copyRectStateQueue.front();
			_copyRectStateQueue.pop();

			Assert::AreEqual((const void*)buffer, copyRectState.buf);
			Assert::AreEqual(expectedPitch, copyRectState.pitch);
			Assert::AreEqual(expectedX, copyRectState.x);
			Assert::AreEqual(expectedY, copyRectState.y);
			Assert::AreEqual(expectedWidth, copyRectState.w);
			Assert::AreEqual(expectedHeight, copyRectState.h);
			Assert::AreEqual(keyColor, copyRectState.ignore);
			Assert::AreEqual(isMouseUpdate, copyRectState.isMouseUpdate);
			Assert::AreEqual(noUpdates, copyRectState.noUpdates);
			Assert::AreEqual(callOrder, copyRectState.callOrder);
		}

		void MouseStateUpdatedCorrectly(int expectedX, int expectedY, int expectedWidth, int expectedHeight, int previousWidth, int previousHeight)
		{
			MouseStateUpdatedCorrectly(expectedX, expectedY, expectedWidth, expectedHeight, previousWidth, previousHeight, expectedWidth, expectedHeight);
		}

		void MouseStateUpdatedCorrectly(int expectedX, int expectedY, int expectedWidth, int expectedHeight, int previousWidth, int previousHeight, int expectedFullWidth, int expectedFullHeight)
		{
			MouseState mouseState = _graphicsManager.getMouseState();
			Assert::AreEqual((const void*)&mouseBuffer, mouseState.buffer);

			for (int i = 0, j = 0; i < NO_IN_MOUSE_PALLETTE; i += 3, j++)
			{
				Assert::AreEqual(mousePallette[i], (byte)mouseState.cursorPallette[j].r);
				Assert::AreEqual(mousePallette[i + 1], (byte)mouseState.cursorPallette[j].g);
				Assert::AreEqual(mousePallette[i + 2], (byte)mouseState.cursorPallette[j].b);
				Assert::AreEqual((byte)255, (byte)mouseState.cursorPallette[j].a);
			}

			Assert::AreEqual(expectedFullWidth, mouseState.fullWidth);
			Assert::AreEqual(expectedFullHeight, mouseState.fullHeight);
			Assert::AreEqual(expectedHeight, mouseState.height);
			Assert::AreEqual(KEY_COLOR, mouseState.keyColor);
			Assert::AreEqual(expectedHeight, mouseState.prevH);
			Assert::AreEqual(expectedWidth, mouseState.prevW);
			Assert::AreEqual(expectedX, mouseState.prevX);
			Assert::AreEqual(expectedY, mouseState.prevY);
			Assert::AreEqual(expectedWidth, mouseState.width);
			Assert::AreEqual(expectedX, mouseState.x);
			Assert::AreEqual(expectedY, mouseState.y);

		}

		void AssertMouseNotMoved(MouseState mouseBefore, MouseState mouseAfter)
		{
			Assert::AreEqual(mouseBefore.fullHeight, mouseAfter.fullHeight);
			Assert::AreEqual(mouseBefore.fullWidth, mouseAfter.fullWidth);
			Assert::AreEqual(mouseBefore.height, mouseAfter.height);
			Assert::AreEqual(mouseBefore.keyColor, mouseAfter.keyColor);
			Assert::AreEqual(mouseBefore.prevH, mouseAfter.prevH);
			Assert::AreEqual(mouseBefore.prevW, mouseAfter.prevW);
			Assert::AreEqual(mouseBefore.prevX, mouseAfter.prevX);
			Assert::AreEqual(mouseBefore.prevY, mouseAfter.prevY);
			Assert::AreEqual(mouseBefore.width, mouseAfter.width);
			Assert::AreEqual(mouseBefore.x, mouseAfter.y);

			Assert::IsTrue(_blotStateQueue.empty());
			Assert::IsTrue(_copyRectStateQueue.empty());
		}
		static void CheckQueuesAreEmpty()
		{
			Assert::IsTrue(_blotStateQueue.empty());
			Assert::IsTrue(_copyRectStateQueue.empty());
		}
		static void ResetTestState()
		{
			CheckQueuesAreEmpty();
			TestCustomScummVMSubclasses::_callOrder = -1;
		}
	};

	//Static instance variables must be declared twice in c++
	byte TestCustomScummVMSubclasses::mouseBuffer[NO_IN_MOUSE_BUFFER];
	byte TestCustomScummVMSubclasses::pictureBuffer[NO_IN_PIC_BUFFER];
	byte TestCustomScummVMSubclasses::mousePallette[NO_IN_MOUSE_PALLETTE];
	byte TestCustomScummVMSubclasses::pallette[NO_IN_PALLETTE];

	std::queue<TestCustomScummVMSubclasses::CopyRectState> TestCustomScummVMSubclasses::_copyRectStateQueue;
	std::queue <TestCustomScummVMSubclasses::BlotState> TestCustomScummVMSubclasses::_blotStateQueue;
	int  TestCustomScummVMSubclasses::_callOrder;
}
