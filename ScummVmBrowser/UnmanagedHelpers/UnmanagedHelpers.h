#pragma once
#include "../../common/scummsys.h"
#include "../../common/stream.h"
#include <vector>;
namespace UnmanagedHelpers {
class ScummByteStream : public Common::WriteStream {
public:
	ScummByteStream();
	~ScummByteStream();
	std::vector<byte> *getData();

protected:
	uint32 write(const void *dataPtr, uint32 dataSize) override;
	int32 pos() const override;

	std::vector<byte>* _data;
};
} // namespace UnmanagedHelpers
