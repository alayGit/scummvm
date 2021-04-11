#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "GetSaveManager.h"

Common::SaveFileManager *SaveManager::GetSaveManager::GetSaveFileManager(ISaveCache ^ saveCache, void* saveData, IByteEncoder^ byteEncoder) {
	return new SaveManager::UnmanagedSaveManagerWrapper(saveCache, static_cast <f_SaveFileData>(saveData), byteEncoder);
}
