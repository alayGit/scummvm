#pragma once
#include "../nativeScummWrapper/scummVm.h"
#include <msclr/gcroot.h>

using namespace Common;
using namespace System::Collections::Generic;
using namespace ManagedCommon::Models;
using namespace ManagedCommon::Interfaces;
using namespace msclr;
namespace SaveManager {
class UnmanagedSaveManagerWrapper : public SaveFileManager {
public:
	UnmanagedSaveManagerWrapper(ISaveCache^ saveCache);
	virtual OutSaveFile *openForSaving(const Common::String &name, bool compress = true);
	virtual InSaveFile *openForLoading(const Common::String &name);
	virtual InSaveFile *openRawFile(const Common::String &name);
	virtual bool removeSavefile(const Common::String &name);
	virtual StringArray listSavefiles(const Common::String &pattern);
	virtual void updateSavefilesList(StringArray &lockedFiles);
	void setGameSaveCache(Dictionary<System::String^, GameSave^>^ *cache);

private:
	gcroot<ISaveCache^> _saveCache;
};
} // namespace SaveManager
