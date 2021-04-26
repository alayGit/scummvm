#pragma once
#include "scummVm.h"
#include "Debugger.h"
#include <functional>
#include <vector>

namespace NativeScummWrapper {
    typedef std::function<Common::String(Common::String input, std::vector<byte>)> AddToCache;
    typedef std::function<void(Common::String input)> RemoveFromCache;

	typedef bool(__stdcall *f_SaveFileData)(Common::String);
	class NativeScummVmWrapperSaveMemStream : public Common::MemoryWriteStreamDynamic {
	private:
		Common::String _fileName;
		int _gameId;
		f_SaveFileData _saveData;

	public:
		AddToCache _addToCache;
	    RemoveFromCache _removeFromCache;
		virtual bool flush() override;
	    NativeScummVmWrapperSaveMemStream(Common::String fileName, f_SaveFileData saveData, AddToCache addToCache, RemoveFromCache removeFromCache);
	};
} // namespace NativeScummWrapper
