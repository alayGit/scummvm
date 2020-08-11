#pragma once
#include "../../common/str.h"

using namespace System;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

namespace CLIScummConverterTests
{
	[TestClassAttribute]
	public ref class UtilityTests
	{
	public:
		[TestMethod]
		void ManagedStringToCommonStringCorrectConverts();
	};
}
