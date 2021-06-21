#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "screenCache.h"

uint32 NativeScummWrapper::ScreenCache::AddScreenToCache(ScreenBuffer screenBuffer) {
	uint32 hash = CalculateHash(screenBuffer);
	_order[hash]++;

	_screenBuffers[hash] = screenBuffer;

	return hash;
}

bool NativeScummWrapper::ScreenCache::IsInCache(ScreenBuffer screenBuffer) {
	return false;
}

uint32 NativeScummWrapper::ScreenCache::CalculateHash(ScreenBuffer screenBuffer) {
	std::string hashInput = std::to_string(screenBuffer.w) + std::to_string(screenBuffer.w)
		+ std::to_string(screenBuffer.h)
		+ std::to_string(screenBuffer.x)
	    + std::to_string(screenBuffer.y);

	for (int i = 0; i < screenBuffer.length; i++)
	{
		hashInput = hashInput + std::to_string(screenBuffer.buffer[i]);
	}

	return std::hash<std::string>()(hashInput);
}
