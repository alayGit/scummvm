#include "GetThumbnail.h"

std::vector<byte> *SaveManager::GetThumbnail::getThumbnail(GraphicsManager *graphics, NativeScummWrapper::NativeScummWrapperPaletteManager * paletteManager) {

	std::vector<byte> *result = new std::vector<byte>();
	result->resize(THUMBNAIL_DEFAULT_WIDTH * THUMBNAIL_DEFAULT_HEIGHT * graphics->getScreenFormat().bytesPerPixel);

	Graphics::Surface* currentScreen = graphics->lockScreen();
	Graphics::Surface *smallerScreen = currentScreen->scale(THUMBNAIL_DEFAULT_WIDTH, THUMBNAIL_DEFAULT_HEIGHT);

	memcpy(&(*result)[0], smallerScreen->getPixels(), result->size());

	graphics->unlockScreen();

	smallerScreen->free();
	delete smallerScreen;

	return result;
}
