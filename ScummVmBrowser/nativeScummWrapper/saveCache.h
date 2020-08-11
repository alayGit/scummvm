#pragma once

#include <vector>
#include "scummvm.h"
#include "hashMap.h";

namespace NativeScummWrapper {
	class SaveCache {
	public:
		static void ReplaceCache(SaveFileCache *newCache);
		static void AddToCache(Common::String saveName, std::vector<byte> data);
		static Common::StringArray ListSaves(const Common::String &pattern);
		static std::vector<byte> *GetSaveData(const Common::String &saveName);

	private:
		static SaveFileCache _saveFileCache;
	};
} // namespace NativeScummWrapper
