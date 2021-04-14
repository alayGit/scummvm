#pragma once

#include <vector>
#include "../../ScummVmBrowser/ScummToManagedMarshalling/ScummToManagedMarshalling.h"

using namespace ScummToManagedMarshalling;

namespace ScummToManagedMarshallingTests {
public ref class ConverterTests
	{
	public:
	    static bool CanConvertBetweenManagedAndCommonString();
	    static cli::array<int> ^ CanConvertBetweenVectorAndManagedArray(cli::array<int> ^ array);
	};
}
