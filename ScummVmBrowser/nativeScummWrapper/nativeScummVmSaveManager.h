#pragma once
#include "scummVm.h"
#include "nativeScummVmWrapperSaveMemStream.h"

using namespace Common;

namespace NativeScummWrapper {
	class NativeScummVmSaveManager : public Common::SaveFileManager {
	public:
	    NativeScummWrapper::NativeScummVmSaveManager(NativeScummWrapper::f_SaveFileData saveData);
		virtual OutSaveFile *openForSaving(const Common::String &name, bool compress = true);
		virtual InSaveFile *openForLoading(const Common::String &name);
		virtual InSaveFile *openRawFile(const Common::String &name);
		virtual bool removeSavefile(const Common::String &name);
		virtual StringArray listSavefiles(const Common::String &pattern);
		virtual void updateSavefilesList(StringArray &lockedFiles);
		void setGameSaveCache(SaveFileCache *cache);

	private:
		int _gameId;
		f_SaveFileData _saveData;
	};
} // namespace NativeScummWrapper
