#pragma once

#include <vector>
#include "../../common/str.h"
#include "../../common/hashmap.h"
#include "../nativeScummWrapper/hashMap.h"
#pragma make_public(Common::String)

using namespace System::Runtime::InteropServices;

namespace Utilities
{
	public ref class Converters abstract sealed
	{
	public:
		static Common::String ManagedStringToCommonString(System::String^ input);

		static SaveFileCache* CreateSaveFileCacheFromDictionary(System::Collections::Generic::Dictionary<System::String^, cli::array<System::Byte>^>^ dictionary);
	};
}

