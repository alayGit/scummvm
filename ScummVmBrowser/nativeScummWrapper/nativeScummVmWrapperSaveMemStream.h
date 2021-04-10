#pragma once
#include "scummVm.h"
#include "Debugger.h"
#include <functional>
#include <vector>

namespace NativeScummWrapper {
    typedef std::function<void(Common::String input, std::vector<byte>)> AddToCache;
	typedef bool(__stdcall *f_SaveFileData)(const byte *, int, Common::String);
	class NativeScummVmWrapperSaveMemStream : public Common::MemoryWriteStreamDynamic {
	private:
		Common::String _fileName;
		int _gameId;
		f_SaveFileData _saveData;

	public:
		AddToCache _addToCache;
		virtual bool flush() override;
	    NativeScummVmWrapperSaveMemStream(Common::String fileName, f_SaveFileData saveData, AddToCache addToCache);
	};
} // namespace NativeScummWrapper
