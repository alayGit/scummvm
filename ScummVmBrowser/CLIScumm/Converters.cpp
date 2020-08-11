#include "Converters.h"

Common::String Utilities::Converters::ManagedStringToCommonString(System::String^ input)
{
	char* p = (char*)Marshal::StringToHGlobalAnsi(input).ToPointer();
	Common::String result = Common::String(p);
	Marshal::FreeHGlobal(System::IntPtr(p));
	
	return result;
}

SaveFileCache* Utilities::Converters::CreateSaveFileCacheFromDictionary(System::Collections::Generic::Dictionary<System::String^, cli::array<System::Byte>^>^ dictionary)
{
	SaveFileCache* result = new SaveFileCache();
	
	if (dictionary != nullptr)
	{
		for each (System::String ^ sskey in dictionary->Keys)
		{
			Common::String cmKey = Utilities::Converters::ManagedStringToCommonString(sskey);

			(*result)[cmKey] = std::vector<byte>(dictionary[sskey]->Length);

			Marshal::Copy(dictionary[sskey], 0, (System::IntPtr) &((*result)[cmKey][0]), dictionary[sskey]->Length);
		}
	}
		return result;
}



