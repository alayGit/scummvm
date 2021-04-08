#include "pch.h"

#include "UnmanagedSaveManagerWrapper.h"

SaveManager::UnmanagedSaveManagerWrapper::UnmanagedSaveManagerWrapper(ISaveCache ^ saveCache) {
	_saveCache = gcroot<ISaveCache ^>(saveCache);
}

OutSaveFile *SaveManager::UnmanagedSaveManagerWrapper::openForSaving(const Common::String &name, bool compress) {
	return nullptr;
}

InSaveFile *SaveManager::UnmanagedSaveManagerWrapper::openForLoading(const Common::String &name) {
	return nullptr;
}

InSaveFile *SaveManager::UnmanagedSaveManagerWrapper::openRawFile(const Common::String &name) {
	return nullptr;
}

bool SaveManager::UnmanagedSaveManagerWrapper::removeSavefile(const Common::String &name) {
	return nullptr;
}

StringArray SaveManager::UnmanagedSaveManagerWrapper::listSavefiles(const Common::String &pattern) {
	return StringArray();
}

void SaveManager::UnmanagedSaveManagerWrapper::updateSavefilesList(StringArray &lockedFiles) {
}

void SaveManager::UnmanagedSaveManagerWrapper::setGameSaveCache(Dictionary<System::String ^, GameSave ^> ^ *cache) {
}
