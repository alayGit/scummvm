#include "GetThumbnail.h"

std::vector<byte> *SaveManager::GetThumbnail::getThumbnail(GraphicsManager *graphics, NativeScummWrapper::NativeScummWrapperPaletteManager * paletteManager) {

	std::vector<byte> *result = new std::vector<byte>();
	result->resize(DISPLAY_DEFAULT_WIDTH * DISPLAY_DEFAULT_HEIGHT * graphics->getScreenFormat().bytesPerPixel);

	Graphics::Surface* currentScreen = graphics->lockScreen();
	Graphics::Surface *x = currentScreen->scale(100, 160);

	memcpy(&(*result)[0], currentScreen->getPixels(), result->size());

	graphics->unlockScreen();

	return result;
}
