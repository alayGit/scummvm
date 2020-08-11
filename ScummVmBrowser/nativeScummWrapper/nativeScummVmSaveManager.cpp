#define FORBIDDEN_SYMBOL_ALLOW_ALL

#include "nativeScummVmSaveManager.h";

 NativeScummWrapper::NativeScummVmSaveManager::NativeScummVmSaveManager(NativeScummWrapper::f_SaveFileData saveData) {
	_saveData = saveData;
}

Common::OutSaveFile *NativeScummWrapper::NativeScummVmSaveManager::openForSaving(const Common::String &name, bool compress) {
	return new Common::OutSaveFile(new NativeScummVmWrapperSaveMemStream(name, _saveData));
}

InSaveFile *NativeScummWrapper::NativeScummVmSaveManager::openForLoading(const String &name) {
	std::vector<byte> *data = NativeScummWrapper::SaveCache::GetSaveData(name);
	
	if (data->size() > 0)
	{
		return new MemoryReadStream(&(data->at(0)), data->size());
	}
	else
	{
		return nullptr;
	}
}

InSaveFile *NativeScummWrapper::NativeScummVmSaveManager::openRawFile(const String &name) {
	throw "Not implemented";
}

bool NativeScummWrapper::NativeScummVmSaveManager::removeSavefile(const String &name) {
	throw "Not implemented";
}

StringArray NativeScummWrapper::NativeScummVmSaveManager::listSavefiles(const String &pattern) {
	return NativeScummWrapper::SaveCache::ListSaves(pattern);
}

void NativeScummWrapper::NativeScummVmSaveManager::updateSavefilesList(StringArray &lockedFiles) {
	throw "Not implemented";
}

void NativeScummWrapper::NativeScummVmSaveManager::setGameSaveCache(SaveFileCache *cache) {
	NativeScummWrapper::SaveCache::ReplaceCache(cache);
}
