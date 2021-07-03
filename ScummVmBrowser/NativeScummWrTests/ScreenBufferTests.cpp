#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "../nativeScummWrapper/screenCache.h"
#include "../nativeScummWrapper/nativeScummWrapperOptions.h"
#include "gtest/gtest.h"
#include <string>

using namespace NativeScummWrapper;

TEST(AddingMoreThanScreenBufferCacheSizeCausesOldestEntryToBeRemoved) {
	const int SCREEN_CACHE_SIZE = 100;

	NativeScummWrapperOptions options;
	options.ScreenBufferCacheSize = SCREEN_CACHE_SIZE;
	ScreenCache screenBuffer = ScreenCache(options);

	const int LENGTH = 5;
	byte buf[SCREEN_CACHE_SIZE -1][LENGTH];

	for (int i = 0; i < SCREEN_CACHE_SIZE - 1; i++) {
		for (int y = 0; y < LENGTH; y++) {
			buf[i][y] = i;
		}
		screenBuffer.AddScreenToCache(buf[i], LENGTH);
	}

	ScreenCacheAddResult screenBufferResult1 = screenBuffer.AddScreenToCache(buf[0], LENGTH);

	EXPECT_FALSE(screenBufferResult1.firstTimeAdded);
	EXPECT_TRUE(screenBufferResult1.hash.length() > 0);

	byte screenBuffer2[LENGTH] = {1, 2, 3, 4, 5};
	ScreenCacheAddResult screenBufferAfterFull = screenBuffer.AddScreenToCache(screenBuffer2, LENGTH);
	EXPECT_TRUE(screenBufferAfterFull.firstTimeAdded);
	EXPECT_TRUE(screenBufferAfterFull.hash.length() > 0);

	ScreenCacheAddResult screenBufferResult2 = screenBuffer.AddScreenToCache(buf[0], LENGTH);
	EXPECT_TRUE(screenBufferResult2.firstTimeAdded);
	EXPECT_TRUE(screenBufferResult2.hash.length() > 0);
}

TEST(CanAddHashToScreenCache) {
	NativeScummWrapperOptions options;
	options.ScreenBufferCacheSize = 100;

	ScreenCache screenBuffer = ScreenCache(options);

	const int length = 5;
	byte buf[length] = {1, 23, 55, 123, 11};

	ScreenCacheAddResult firstAdd = screenBuffer.AddScreenToCache(buf, length);
	EXPECT_TRUE(firstAdd.firstTimeAdded);
	EXPECT_TRUE(firstAdd.hash.length() > 0);

	ScreenCacheAddResult secondAdd = screenBuffer.AddScreenToCache(buf, length);
	EXPECT_FALSE(secondAdd.firstTimeAdded);
	EXPECT_EQ(firstAdd.hash, secondAdd.hash);
}
