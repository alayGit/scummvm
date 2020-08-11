#pragma once
#include "scummVm.h"
#include "saveCache.h"
#include "Debugger.h"

namespace NativeScummWrapper {
	typedef bool(__stdcall *f_SaveFileData)(const byte *, int, Common::String);
	class NativeScummVmWrapperSaveMemStream : public MemoryWriteStreamDynamic {
	private:
		Common::String _fileName;
		int _gameId;
		f_SaveFileData _saveData;

	public:
		virtual bool flush() override;
		NativeScummVmWrapperSaveMemStream(Common::String fileName, f_SaveFileData saveData);
	};
} // namespace NativeScummWrapper
