#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "GetSaveManager.h"

Common::SaveFileManager *SaveManager::GetSaveManager::GetSaveFileManager(ISaveCache ^ saveCache) {
	return new SaveManager::UnmanagedSaveManagerWrapper(saveCache);
}
