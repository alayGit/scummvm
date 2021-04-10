#define FORBIDDEN_SYMBOL_ALLOW_ALL

#include "nativeScummVmWrapperSaveMemStream.h"

NativeScummWrapper::NativeScummVmWrapperSaveMemStream::NativeScummVmWrapperSaveMemStream(Common::String fileName, NativeScummWrapper::f_SaveFileData saveData, AddToCache addToCache) : MemoryWriteStreamDynamic(DisposeAfterUse::NO) {
	_fileName = fileName;
	_saveData = saveData;
}

bool NativeScummWrapper::NativeScummVmWrapperSaveMemStream::flush()
{
	std::vector<byte>* dataCopy = new std::vector<byte>();
	for (int i = 0; i < size(); i++)
	{
		dataCopy->push_back(*(getData() + i));
	}
	Common::String s = Common::String(_fileName.c_str());
	
	bool result = MemoryWriteStreamDynamic::flush() && _saveData(&dataCopy->at(0), size(), s);
	if (result)
	{
		SaveCache::AddToCache(_fileName, *dataCopy);
	}
	
	delete dataCopy;

	return result;
}
