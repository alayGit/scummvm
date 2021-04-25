#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "../nativeScummWrapper/PaletteManager.h"
#include "gtest/gtest.h"
#include <string>

class PaletteManagerTest : public ::testing::Test {
public:
	NativeScummWrapper::NativeScummWrapperPaletteManager _paletteManager;
};

byte *getColorByteArrayFromString(const char *s) {
	byte *result = new byte[NO_BYTES_PER_PIXEL * NO_COLOURS];

	for (int i = 0; i < NO_COLOURS * NO_BYTES_PER_PIXEL; i++) {
		char colorComponent[3];

		for (int j = 0; j < NO_DIGITS_IN_PALETTE_VALUE; j++) {
			colorComponent[j] = s[i * NO_DIGITS_IN_PALETTE_VALUE + j];
		}

		result[i] = std::atoi(colorComponent);
	}

	return result;
}

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

	delete[] colors;
}

TEST_F(PaletteManagerTest, IsCorrectlyInitedHaveSeenPalette) {
	EXPECT_FALSE(_paletteManager.haveSeenPalette(_paletteManager.getCurrentPaletteHash()));
}

TEST_F(PaletteManagerTest, IsCorrectlyInitedGetCurrentPicturePaletteHash) {
	NativeScummWrapper::PalletteColor palettes[NO_COLOURS];

	for (int i = 0; i < NO_COLOURS; i++) {
		palettes[i].a = i;
		palettes[i].r = i;
		palettes[i].g = i;
		palettes[i].b = i;
	}

	uint32 paletteHash = _paletteManager.RememberPalette(&palettes[0], NO_COLOURS);
	_paletteManager.setCurrentPaletteHash(paletteHash);

	EXPECT_EQ(paletteHash, _paletteManager.getCurrentPaletteHash());
}

TEST_F(PaletteManagerTest, IsCorrectlyInitedGetCurrentCursorPaletteHash) {
	_paletteManager.setCursorPaletteDisabled(true);
	NativeScummWrapper::PalletteColor palettes[NO_COLOURS];

	for (int i = 0; i < NO_COLOURS; i++) {
		palettes[i].a = i;
		palettes[i].r = i;
		palettes[i].g = i;
		palettes[i].b = i;
	}

	uint32 paletteHash = _paletteManager.RememberPalette(&palettes[0], NO_COLOURS);
	_paletteManager.setCurrentPaletteHash(paletteHash);

	EXPECT_EQ(paletteHash, _paletteManager.getCurrentCursorPaletteHash());
}

TEST_F(PaletteManagerTest, IsCorrectlyInitedPicturePalette) {
	const int NO_NEW_COLORS = 10;
	const int START_NEW_COLORS = 20;
	byte newColors[NO_NEW_COLORS * NO_COLOUR_COMPONENTS_SCUMM_VM];

	byte *initialPaletteColor = getColorByteArrayFromString(_paletteManager.getPalette(_paletteManager.getCurrentPaletteHash()));
	for (int i = 0; i < NO_COLOURS; i++) {
		ASSERT_EQ(initialPaletteColor[i * NO_BYTES_PER_PIXEL], 0);
		ASSERT_EQ(initialPaletteColor[i * NO_BYTES_PER_PIXEL + 1], 0);
		ASSERT_EQ(initialPaletteColor[i * NO_BYTES_PER_PIXEL + 2], 0);
		ASSERT_EQ(initialPaletteColor[i * NO_BYTES_PER_PIXEL + 3], 255);
	}
}

TEST_F(PaletteManagerTest, IsCorrectlyInitedCursorPalette) {
	_paletteManager.setCursorPaletteDisabled(true);
	const int NO_NEW_COLORS = 10;
	const int START_NEW_COLORS = 20;
	byte newColors[NO_NEW_COLORS * NO_COLOUR_COMPONENTS_SCUMM_VM];

	byte *initialPaletteColor = getColorByteArrayFromString(_paletteManager.getPalette(_paletteManager.getCurrentCursorPaletteHash()));
	for (int i = 0; i < NO_COLOURS; i++) {
		ASSERT_EQ(initialPaletteColor[i * NO_BYTES_PER_PIXEL], 0);
		ASSERT_EQ(initialPaletteColor[i * NO_BYTES_PER_PIXEL + 1], 0);
		ASSERT_EQ(initialPaletteColor[i * NO_BYTES_PER_PIXEL + 2], 0);
		ASSERT_EQ(initialPaletteColor[i * NO_BYTES_PER_PIXEL + 3], 255);
	}
}

TEST_F(PaletteManagerTest, ExpectionSetCurrentCursorPaletteHashWhenPaletteDoesNotExist) {
	EXPECT_ANY_THROW(_paletteManager.setCurrentCursorPaletteHash(7));
}

TEST_F(PaletteManagerTest, CanRememberAndRetrievePalette) {
	NativeScummWrapper::PalletteColor palettes[NO_COLOURS];

	for (int i = 0; i < NO_COLOURS; i++) {
		palettes[i].a = i;
		palettes[i].r = i;
		palettes[i].g = i;
		palettes[i].b = i;
	}

	uint32 paletteHash = _paletteManager.RememberPalette(&palettes[0], NO_COLOURS);
	_paletteManager.setCurrentPaletteHash(paletteHash);

	EXPECT_EQ(paletteHash, _paletteManager.getCurrentCursorPaletteHash());
}

TEST_F(PaletteManagerTest, CanCreateNewPaletteBasedOnPicturePalette) {
	const int NO_NEW_COLORS = 10;
	const int START_NEW_COLORS = 20;
	byte newColors[NO_NEW_COLORS * NO_COLOUR_COMPONENTS_SCUMM_VM];

	for (int i = 0; i < NO_NEW_COLORS * NO_COLOUR_COMPONENTS_SCUMM_VM; i++) {
		newColors[i] = i;
	}

	uint32 paletteHash = _paletteManager.createNewPaletteBasedOnPicturePalette(&newColors[0], START_NEW_COLORS, NO_NEW_COLORS);
	_paletteManager.setCurrentPaletteHash(paletteHash);

	byte *newPaletteColor = getColorByteArrayFromString(_paletteManager.getPalette(_paletteManager.getCurrentPaletteHash()));
	for (int i = 0; i < NO_COLOURS; i++) {

		if (i < START_NEW_COLORS || i >= START_NEW_COLORS + NO_NEW_COLORS) {
			ASSERT_EQ(newPaletteColor[i * NO_BYTES_PER_PIXEL], 0);
			ASSERT_EQ(newPaletteColor[i * NO_BYTES_PER_PIXEL + 1], 0);
			ASSERT_EQ(newPaletteColor[i * NO_BYTES_PER_PIXEL + 2], 0);
			ASSERT_EQ(newPaletteColor[i * NO_BYTES_PER_PIXEL + 3], 255);
		} else {
			ASSERT_EQ(newPaletteColor[i * NO_BYTES_PER_PIXEL], newColors[(i - START_NEW_COLORS) * NO_COLOUR_COMPONENTS_SCUMM_VM]);
			ASSERT_EQ(newPaletteColor[i * NO_BYTES_PER_PIXEL + 1], newColors[(i - START_NEW_COLORS) * NO_COLOUR_COMPONENTS_SCUMM_VM + 1]);
			ASSERT_EQ(newPaletteColor[i * NO_BYTES_PER_PIXEL + 2], newColors[(i - START_NEW_COLORS) * NO_COLOUR_COMPONENTS_SCUMM_VM + 2]);
			ASSERT_EQ(newPaletteColor[i * NO_BYTES_PER_PIXEL + 3], 255);
		}
	}
}

TEST_F(PaletteManagerTest, CursorPaletteAndPicturePaletteNotEqualWhenCursorPaletteEnabled) {
	_paletteManager.setCursorPaletteDisabled(false);
	NativeScummWrapper::PalletteColor palettes[NO_COLOURS];

	for (int i = 0; i < NO_COLOURS; i++) {
		palettes[i].a = i;
		palettes[i].r = i;
		palettes[i].g = i;
		palettes[i].b = i;
	}

	uint32 paletteHash = _paletteManager.RememberPalette(&palettes[0], NO_COLOURS);
	_paletteManager.setCurrentPaletteHash(paletteHash);

	EXPECT_NE(_paletteManager.getCurrentPaletteHash(), _paletteManager.getCurrentCursorPaletteHash());
	EXPECT_NE(_paletteManager.getPalette(_paletteManager.getCurrentPaletteHash()), _paletteManager.getPalette(_paletteManager.getCurrentCursorPaletteHash()));
}

TEST_F(PaletteManagerTest, CursorPaletteAndPicturePaletteEqualWhenCursorPaletteDisabled) {
	NativeScummWrapper::PalletteColor palettes[NO_COLOURS];

	for (int i = 0; i < NO_COLOURS; i++) {
		palettes[i].a = i;
		palettes[i].r = i;
		palettes[i].g = i;
		palettes[i].b = i;
	}

	uint32 paletteHash = _paletteManager.RememberPalette(&palettes[0], NO_COLOURS);
	_paletteManager.setCurrentPaletteHash(paletteHash);

	EXPECT_EQ(_paletteManager.getCurrentPaletteHash(), _paletteManager.getCurrentCursorPaletteHash());
	EXPECT_EQ(_paletteManager.getPalette(_paletteManager.getCurrentPaletteHash()), _paletteManager.getPalette(_paletteManager.getCurrentCursorPaletteHash()));
}

TEST_F(PaletteManagerTest, HaveSeenPaletteReturnsFalseWhenPaletteNotSeen) {
	EXPECT_FALSE(_paletteManager.haveSeenPalette(_paletteManager.getCurrentCursorPaletteHash()));
}

TEST_F(PaletteManagerTest, HaveSeenPaletteReturnsTrueWhenPaletteSeen) {
	_paletteManager.registerSeenPalette(_paletteManager.getCurrentCursorPaletteHash());
	EXPECT_TRUE(_paletteManager.haveSeenPalette(_paletteManager.getCurrentCursorPaletteHash()));
}

TEST_F(PaletteManagerTest, ThrowsIfTryingToRegisterSeenPaletteWhichIsUnknown) {
	EXPECT_ANY_THROW(_paletteManager.registerSeenPalette(55));
}
