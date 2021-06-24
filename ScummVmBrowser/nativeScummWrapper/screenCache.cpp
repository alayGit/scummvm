#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "screenCache.h"

NativeScummWrapper::ScreenCacheAddResult NativeScummWrapper::ScreenCache::AddScreenToCache(const byte *buf, int length) {
	NativeScummWrapper::ScreenCacheAddResult result;
	result.hash = CalculateHash(buf, length);
	result.firstTimeAdded = !_screenBuffers[result.hash];

	_screenBuffers[result.hash] = true;

	return result;
}


std::string NativeScummWrapper::ScreenCache::CalculateHash(const byte *buf, int length) {
	SHA1 sha1;

	return sha1(buf, length);
}
