#pragma once
#include "../../common/scummsys.h"
#include "../../common/stream.h"
#include <vector>;
namespace UnmanagedHelpers {
class ScummByteStream : Common::WriteStream {
	std::vector<byte> _stream;
	uint32 write(const void *dataPtr, uint32 dataSize) override;

	std::vector<byte> getStream();
};
} // namespace UnmanagedHelpers
