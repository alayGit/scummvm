#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "UnmanagedSaveManagerWrapper.h"

SaveManager::UnmanagedSaveManagerWrapper::UnmanagedSaveManagerWrapper(ISaveCache ^ saveCache, f_SaveFileData saveData, ISaveDataEncoder ^ yEncoder, NativeScummWrapperPaletteManager *paletteManager, GraphicsManager *graphics) {
	_saveCache = gcroot<ISaveCache ^>(saveCache);
	_saveData = saveData;
	_encoder = yEncoder;
	_paletteManager = paletteManager;
	_graphics = graphics;
}

OutSaveFile *SaveManager::UnmanagedSaveManagerWrapper::openForSaving(const Common::String &name, bool compress) {
	return new Common::OutSaveFile(new NativeScummVmWrapperSaveMemStream(
	    name, _saveData, [this](Common::String name, std::vector<byte> saveData)
		{
 			System::String ^ managedFileName = Converters::CommonStringToManagedString(&name);
			cli::array<byte> ^ managedSaveData = ScummToManagedMarshalling::Converters::MarshalVectorToManagedArray(&saveData, false);

			_saveCache->SaveToCache(managedFileName, getGameSave(managedSaveData));
	
			return Converters::ManagedStringToCommonString(_saveCache->GetCompressedAndEncodedSaveData());
		},


		[this](const Common::String name) {
		    _saveCache->RemoveFromCache(Converters::CommonStringToManagedString(&name));
	    }));
}

InSaveFile *SaveManager::UnmanagedSaveManagerWrapper::openForLoading(const Common::String &name) {
	array<byte> ^ managedSaveData = _saveCache->GetFromCache(Converters::CommonStringToManagedString(&name));

	if (managedSaveData != nullptr && managedSaveData->Length > 0) {
		byte *unmanagedSaveData = new byte[managedSaveData->Length];
		Marshal::Copy(managedSaveData, 0, System::IntPtr(unmanagedSaveData), managedSaveData->Length);

		return new MemoryReadStream(unmanagedSaveData, managedSaveData->Length, DisposeAfterUse::YES);
	} else {
		return nullptr;
	}
}

InSaveFile *SaveManager::UnmanagedSaveManagerWrapper::openRawFile(const Common::String &name) {
	return nullptr;
}

bool SaveManager::UnmanagedSaveManagerWrapper::removeSavefile(const Common::String &name) {
	_saveCache->RemoveFromCache(Converters::CommonStringToManagedString(&name));
	System::String^ managedStringCompressedAndEncodedSaveData = _saveCache->GetCompressedAndEncodedSaveData();

	bool result = _saveData(Converters::ManagedStringToCommonString(managedStringCompressedAndEncodedSaveData));

	return result;
}

StringArray SaveManager::UnmanagedSaveManagerWrapper::listSavefiles(const Common::String &pattern) {
	StringArray result;
	IEnumerable<System::String ^> ^ managedSaveFileNames = _saveCache->ListCache();

	for each(System::String ^ managedSaveFileName in managedSaveFileNames) {
			result.push_back(Converters::ManagedStringToCommonString(managedSaveFileName));
		}

	return result;
}

void SaveManager::UnmanagedSaveManagerWrapper::updateSavefilesList(StringArray &lockedFiles) {
}

void SaveManager::UnmanagedSaveManagerWrapper::setGameSaveCache(System::String ^ yEncodedCompressedCache) {
	_saveCache->SetCache(yEncodedCompressedCache);
}

GameSave ^ SaveManager::UnmanagedSaveManagerWrapper::getGameSave(cli::array<byte> ^ saveData) {
	ManagedCommon::Models::GameSave ^ gameSave = gcnew ManagedCommon::Models::GameSave();
	gameSave->Data = saveData;
	gameSave->Thumbnail = Converters::MarshalVectorToManagedArray(SaveManager::GetThumbnail::getThumbnail(_graphics, _paletteManager));
	gameSave->PaletteString = gcnew System::String(_paletteManager->getPalette(_paletteManager->getCurrentPaletteHash()));

	return gameSave;
}
