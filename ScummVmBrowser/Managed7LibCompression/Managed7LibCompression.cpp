#include "Managed7LibCompression.h"

cli::array<System::Byte> ^ SevenZCompression::SevenZCompressor::Compress(cli::array<System::Byte> ^ data) {
	std::vector<System::Byte> inputVector;
	inputVector.resize(data->Length);

	Marshal::Copy(data, 0, System::IntPtr(&inputVector[0]), inputVector.size());

	std::vector<System::Byte> outputVector;

	SevenZCompression::Compress(outputVector, inputVector);

	cli::array<System::Byte> ^ result = gcnew cli::array<System::Byte>(outputVector.size());
	Marshal::Copy(System::IntPtr(&outputVector[0]), result, 0, outputVector.size());

	return result;
}

cli::array<System::Byte> ^ SevenZCompression::SevenZCompressor::Decompress(cli::array<System::Byte> ^ data) {
	std::vector<System::Byte> inputVector;
	inputVector.resize(data->Length);

	Marshal::Copy(data, 0, System::IntPtr(&inputVector[0]), inputVector.size());

	std::vector<System::Byte> outputVector;

	SevenZCompression::Uncompress(outputVector, inputVector);

	cli::array<System::Byte> ^ result = gcnew cli::array<System::Byte>(outputVector.size());
	Marshal::Copy(System::IntPtr(&outputVector[0]), result, 0, outputVector.size());

	return result;
}
