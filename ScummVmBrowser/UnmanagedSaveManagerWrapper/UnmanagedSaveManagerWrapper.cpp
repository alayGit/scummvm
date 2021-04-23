#define FORBIDDEN_SYMBOL_ALLOW_ALL
#include "UnmanagedSaveManagerWrapper.h"

SaveManager::UnmanagedSaveManagerWrapper::UnmanagedSaveManagerWrapper(ISaveCache ^ saveCache, f_SaveFileData saveData, IByteEncoder ^ yEncoder, NativeScummWrapperPaletteManager *paletteManager, GraphicsManager *graphics) {
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
			cli::array<byte> ^ managedSaveData = gcnew cli::array<byte>(saveData.size());

			Marshal::Copy(System::IntPtr(&saveData[0]), managedSaveData, 0, managedSaveData->Length);

			ManagedCommon::Models::GameSave ^ gameSave = gcnew ManagedCommon::Models::GameSave();
			gameSave->Data = managedSaveData;
		    gameSave->Thumbnail = Converters::MarshalVectorToManagedArray(SaveManager::GetThumbnail::getThumbnail(_graphics, _paletteManager));
		    gameSave->PaletteString = gcnew System::String(_paletteManager->getPalette());

			_saveCache->SaveToCache(managedFileName, gameSave);

			cli::array<byte>^ managedSaveDataBytes = _encoder->TextEncoding->GetBytes(_saveCache->GetCompressedAndEncodedSaveData());
			std::vector<byte>* unmanagedSaveDataBytes = new std::vector<byte>();
		    unmanagedSaveDataBytes->resize(managedSaveDataBytes->Length);

			Marshal::Copy(managedSaveDataBytes, 0, System::IntPtr(&(*unmanagedSaveDataBytes)[0]), managedSaveDataBytes->Length);

			return unmanagedSaveDataBytes;
		},


		[this](const Common::String name) {
		    _saveCache->RemoveFromCache(Converters::CommonStringToManagedString(&name));
	    }));
}

InSaveFile *SaveManager::UnmanagedSaveManagerWrapper::openForLoading(const Common::String &name) {
	array<byte> ^ managedSaveData = _saveCache->GetFromCache(Converters::CommonStringToManagedString(&name));

	if (managedSaveData->Length > 0) {
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

	byte *unmanagedCompressedAndEncodedSaveData = ScummToManagedMarshalling::Converters::MarshalManagedStringToByteArray(managedStringCompressedAndEncodedSaveData, _encoder->TextEncoding);

	bool result = _saveData(unmanagedCompressedAndEncodedSaveData, managedStringCompressedAndEncodedSaveData->Length);

	delete unmanagedCompressedAndEncodedSaveData;

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
