#define FORBIDDEN_SYMBOL_ALLOW_ALL

#include "saveCache.h"

SaveFileCache NativeScummWrapper::SaveCache::_saveFileCache;

void NativeScummWrapper::SaveCache::ReplaceCache(SaveFileCache* newCache)
{
	_saveFileCache = SaveFileCache(*newCache);
}

void NativeScummWrapper::SaveCache::AddToCache(String saveName, std::vector<byte> data) {
	SaveFileCache::const_iterator saveData = _saveFileCache.find(saveName);

	_saveFileCache[saveName] = data;
}

Common::StringArray NativeScummWrapper::SaveCache::ListSaves(const Common::String &pattern) {
	Common::StringArray results;
	for (SaveFileCache::const_iterator saveData = _saveFileCache.begin(), end = _saveFileCache.end(); saveData != end; ++saveData) {
		if (saveData->_key.matchString(pattern, true)) {
			results.push_back(saveData->_key);
		}
	}
	return results;
}

std::vector<byte>* NativeScummWrapper::SaveCache::GetSaveData(const Common::String &saveName) {
	return &_saveFileCache[saveName];
}

