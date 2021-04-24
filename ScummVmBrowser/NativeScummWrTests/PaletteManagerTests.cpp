#include "gtest/gtest.h"
#include "../nativeScummWrapper/PaletteManager.h"

class PaletteManagerTest : public ::testing::Test {
public:
	NativeScummWrapper::NativeScummWrapperPaletteManager _paletteManager;
};

TEST_F(PaletteManagerTest, IsCorrectlyInitedCursorPaletteHash) {
	EXPECT_TRUE(_paletteManager.getCurrentCursorPaletteHash() > 0);
	EXPECT_TRUE(_paletteManager.getCurrentPaletteHash() > 0);
}

TEST_F(PaletteManagerTest, IsCorrectlyInitedCurrentPaletteColor) {
	NativeScummWrapper::PalletteColor *paletteColor = _paletteManager.getCurrentPaletteAsPaletteColor();
	for (int i = 0; i < NO_COLOURS; i++) {
		EXPECT_EQ(paletteColor[i].a, 255);
		EXPECT_EQ(paletteColor[i].r, 0);
		EXPECT_EQ(paletteColor[i].g, 0);
		EXPECT_EQ(paletteColor[i].b, 0);
	}
}

TEST_F(PaletteManagerTest, IsCorrectlyInitedGrabPalette) {
	uint32 currentPaletteHash = _paletteManager.getCurrentPaletteHash();
	byte *colors = new byte[NO_COLOURS];
	_paletteManager.grabPalette(colors, 0, NO_COLOURS);

	for (int i = 0; i < NO_COLOURS; i++) {
		if (i + 1 % 4 == 0) {
			EXPECT_EQ(colors[i], 255);
		} else {
			EXPECT_EQ(colors[i], 0);
		}
	}

	delete colors;
}

TEST_F(PaletteManagerTest, IsCorrectlyInitedHaveSeenPalette) {
	EXPECT_TRUE(_paletteManager.haveSeenPalette(_paletteManager.getCurrentPaletteHash()));
}
