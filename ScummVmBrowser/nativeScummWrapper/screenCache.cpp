#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "screenCache.h"

NativeScummWrapper::ScreenCache::ScreenCache(NativeScummWrapperOptions nativeScummWrapperOptions) {
	_nativeScummWrapperOptions = nativeScummWrapperOptions;
}

NativeScummWrapper::ScreenCacheAddResult NativeScummWrapper::ScreenCache::AddScreenToCache(const byte *buf, int length) {
	NativeScummWrapper::ScreenCacheAddResult result;
	result.hash = CalculateHash(buf, length);
	result.firstTimeAdded = !_screenBuffers[result.hash];

	_screenBuffers[result.hash] = true;

	if (result.firstTimeAdded) {
		_addOrder.push(result.hash);

		if (_addOrder.size() > _nativeScummWrapperOptions.ScreenBufferCacheSize) {
			_screenBuffers.erase(_addOrder.front());
			_addOrder.pop();
		}
	}

	return result;
}


std::string NativeScummWrapper::ScreenCache::CalculateHash(const byte *buf, int length) {
	SHA1 sha1;

	return sha1(buf, length);
}
