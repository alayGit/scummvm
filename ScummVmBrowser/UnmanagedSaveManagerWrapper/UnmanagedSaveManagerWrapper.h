#pragma once
#include "../nativeScummWrapper/nativeScummVmWrapperSaveMemStream.h"
#include "../ScummToManagedMarshalling/ScummToManagedMarshalling.h"
#include "GetThumbnail.h"
#include <msclr/gcroot.h>
using namespace Common;
using namespace System::Collections::Generic;
using namespace ManagedCommon::Models;
using namespace ManagedCommon::Interfaces;
using namespace msclr;
using namespace ScummToManagedMarshalling;
using namespace NativeScummWrapper;
using namespace System::Runtime::InteropServices;
using namespace ManagedCommon::Models;
namespace SaveManager {
class UnmanagedSaveManagerWrapper : public SaveFileManager {

public:
	UnmanagedSaveManagerWrapper(ISaveCache ^ saveCache, f_SaveFileData saveData, IByteEncoder^ encoder);
	virtual OutSaveFile *openForSaving(const Common::String &name, bool compress = true);
	virtual InSaveFile *openForLoading(const Common::String &name);
	virtual InSaveFile *openRawFile(const Common::String &name);
	virtual bool removeSavefile(const Common::String &name);
	virtual StringArray listSavefiles(const Common::String &pattern);
	virtual void updateSavefilesList(StringArray &lockedFiles);
	void setGameSaveCache(System::String^ yEncodedCompressedCache);

private:
	gcroot<ISaveCache^> _saveCache;
	f_SaveFileData _saveData;
	gcroot<IByteEncoder^> _encoder;
};
} // namespace SaveManager
