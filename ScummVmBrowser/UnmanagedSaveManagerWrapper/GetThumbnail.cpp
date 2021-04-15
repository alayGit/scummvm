#include "GetThumbnail.h"

std::vector<byte> *SaveManager::GetThumbnail::getThumbnail() {
	UnmanagedHelpers::ScummByteStream stream = UnmanagedHelpers::ScummByteStream();

	Graphics::saveThumbnail(stream);

	return stream.getData();
}
