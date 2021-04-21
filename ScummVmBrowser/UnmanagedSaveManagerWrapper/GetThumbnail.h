#pragma once
#include <vector>
#include "../../common/scummsys.h"
#include "..\..\graphics\thumbnail.h"
#include "..\UnmanagedHelpers\UnmanagedHelpers.h"
#include "backends/graphics/graphics.h"
#include "graphics/surface.h"
#include "../nativeScummWrapper/common.h"
#include "../nativeScummWrapper/PaletteManager.h"

namespace SaveManager {
	public class GetThumbnail {
	    public:
	        static std::vector<byte> *getThumbnail(GraphicsManager *graphics, NativeScummWrapper::NativeScummWrapperPaletteManager *paletteManager);
	};
}
