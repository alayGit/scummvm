#pragma once
#include "scummVm.h"
#include "common.h"
#include<string>
#include<map>

namespace NativeScummWrapper {
class ScreenCache {
public:
	uint32 AddScreenToCache(ScreenBuffer screenBuffer);
	bool IsInCache(ScreenBuffer screenBuffer);

private:
	std::map<uint32, int> _order;
	std::unordered_map<int, ScreenBuffer> _screenBuffers;
	uint32 CalculateHash(ScreenBuffer screenBuffer);
};
} // namespace NativeScummWrapper
