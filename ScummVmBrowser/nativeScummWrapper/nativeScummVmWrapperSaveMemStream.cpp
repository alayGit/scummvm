#define FORBIDDEN_SYMBOL_ALLOW_ALL

#include "nativeScummVmWrapperSaveMemStream.h"

NativeScummWrapper::NativeScummVmWrapperSaveMemStream::NativeScummVmWrapperSaveMemStream(Common::String fileName, NativeScummWrapper::f_SaveFileData saveData, AddToCache addToCache, RemoveFromCache removeFromCache) : MemoryWriteStreamDynamic(DisposeAfterUse::NO) {
	_fileName = fileName;
	_saveData = saveData;
	_addToCache = addToCache;
	_removeFromCache = removeFromCache;
}

bool NativeScummWrapper::NativeScummVmWrapperSaveMemStream::flush()
{
	std::vector<byte>* dataCopy = new std::vector<byte>();
	for (int i = 0; i < size(); i++)
	{
		dataCopy->push_back(*(getData() + i));
	}
	Common::String fileName = Common::String(_fileName.c_str());
	Common::String saveCacheData = _addToCache(_fileName, *dataCopy);

	bool result = MemoryWriteStreamDynamic::flush() && _saveData(&dataCopy->at(0), size(), fileName);
	if (!result)
	{
		_removeFromCache(_fileName);
	}
	
	delete dataCopy;

	return result;
}
