#pragma once
#include "scummVm.h"
#include "common.h"
#include "../ExternalLibraries/include/sha1.h";
#include <string>
#include <queue>
#include<map>
#include <cstdlib>
#include "nativeScummWrapperOptions.h"

namespace NativeScummWrapper {

	struct ScreenCacheAddResult {
		std::string hash;
	    bool firstTimeAdded = true;
    };

class ScreenCache {
public:

	ScreenCache(NativeScummWrapperOptions nativeScummWrapperOptions);

	ScreenCacheAddResult AddScreenToCache(const byte* buf, int length);

private:
	std::queue<std::string> _addOrder;
	std::unordered_map<std::string, bool> _screenBuffers;
	std::string CalculateHash(const byte *buf, int length);
	NativeScummWrapperOptions _nativeScummWrapperOptions;
};
} // namespace NativeScummWrapper
