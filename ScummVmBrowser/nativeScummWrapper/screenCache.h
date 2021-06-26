#pragma once
#include "scummVm.h"
#include "common.h"
#include "../ExternalLibraries/include/sha1.h";
#include <string>
#include <queue>
#include<map>

namespace NativeScummWrapper {

	struct ScreenCacheAddResult {
		std::string hash;
	    bool firstTimeAdded;
    };

class ScreenCache {
public:
	ScreenCacheAddResult AddScreenToCache(const byte* buf, int length);

private:
	std::queue<std::string> _addOrder;
	std::unordered_map<std::string, bool> _screenBuffers;
	std::string CalculateHash(const byte *buf, int length);
};
} // namespace NativeScummWrapper
