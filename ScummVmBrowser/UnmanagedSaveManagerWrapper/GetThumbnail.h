#pragma once
#include <vector>
#include "../../common/scummsys.h"
#include "..\..\graphics\thumbnail.h"
#include "..\UnmanagedHelpers\UnmanagedHelpers.h"

namespace SaveManager {
	public class GetThumbnail {
	    public:
	        static std::vector<byte> *getThumbnail();
	};
}
