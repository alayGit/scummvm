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
	EXPECT_FALSE(_paletteManager.haveSeenPalette(_paletteManager.getCurrentPaletteHash()));
}

TEST_F(PaletteManagerTest, ExpectionSetCurrentCursorPaletteHashWhenPaletteDoesNotExist) {
	EXPECT_ANY_THROW(_paletteManager.setCurrentCursorPaletteHash(7));
}

TEST_F(PaletteManagerTest, CanRememberAndRetrievePalette) {
	NativeScummWrapper::PalletteColor palettes[NO_COLOURS];

	for (int i = 0; i < NO_COLOURS; i++)
	{
		palettes[i].a = i;
		palettes[i].r = i;
		palettes[i].g = i;
		palettes[i].b = i;
	}

	uint32  paletteHash = _paletteManager.RememberPalette(&palettes[0], NO_COLOURS);
	_paletteManager.setCurrentPaletteHash(paletteHash);

	EXPECT_EQ(paletteHash, _paletteManager.getCurrentCursorPaletteHash());
}

TEST_F(PaletteManagerTest, CanCreateNewPaletteBasedOnPicturePalette)
{
	const int NO_NEW_COLORS = 10;
	const int START_NEW_COLORS = 20;
	byte newColors[NO_NEW_COLORS * NO_COLOUR_COMPONENTS_SCUMM_VM];

	NativeScummWrapper::PalletteColor *initialPaletteColor = _paletteManager.getCurrentPaletteAsPaletteColor();

	for (int i = 0; i < NO_NEW_COLORS * NO_COLOUR_COMPONENTS_SCUMM_VM; i++) {
		newColors[i] = i;
	}

	uint32 paletteHash = _paletteManager.createNewPaletteBasedOnPicturePalette(&newColors[0], START_NEW_COLORS, NO_NEW_COLORS);
	_paletteManager.setCurrentPaletteHash(paletteHash);

	NativeScummWrapper::PalletteColor *newPaletteColor = _paletteManager.getCurrentPaletteAsPaletteColor();
	for (int i = 0; i < NO_COLOURS; i++) {

		if (i < START_NEW_COLORS || i >= START_NEW_COLORS + NO_NEW_COLORS) {
			ASSERT_EQ(initialPaletteColor[i].a, 255);
			ASSERT_EQ(initialPaletteColor[i].r, 0);
			ASSERT_EQ(initialPaletteColor[i].g, 0);
			ASSERT_EQ(initialPaletteColor[i].b, 0);
		} else {
			ASSERT_EQ(newPaletteColor[i].a, 255);
			ASSERT_EQ(newPaletteColor[i].r, newColors[(i - START_NEW_COLORS) * NO_COLOUR_COMPONENTS_SCUMM_VM]);
			ASSERT_EQ(newPaletteColor[i].g, newColors[(i - START_NEW_COLORS) * NO_COLOUR_COMPONENTS_SCUMM_VM + 1]);
			ASSERT_EQ(newPaletteColor[i].b, newColors[(i - START_NEW_COLORS) * NO_COLOUR_COMPONENTS_SCUMM_VM + 2]);
		}
	}
	
}
