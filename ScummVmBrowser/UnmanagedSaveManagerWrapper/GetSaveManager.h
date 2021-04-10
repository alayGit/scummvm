#pragma once
#include "UnmanagedSaveManagerWrapper.h"
#include "common/savefile.h"
#pragma make_public(Common::SaveFileManager)

using namespace ManagedCommon::Interfaces;

namespace SaveManager {
public ref class GetSaveManager {
public:
	static Common::SaveFileManager *GetSaveFileManager(ISaveCache ^ saveCache);
};
} // namespace SaveManager
