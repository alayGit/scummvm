#include "Managed7LibCompression.h"

cli::array<System::Byte> ^ SevenZCompression::SevenZCompressor::Compress(cli::array<System::Byte> ^ data, unsigned int level) {

	byte *input = new byte[data->Length];
	Marshal::Copy(data, 0, System::IntPtr(input), data->Length);

	unsigned int outputLength;
	byte* output = SevenZCompression::Compress(input, data->Length, outputLength, level);

	cli::array<System::Byte> ^ result = gcnew cli::array<System::Byte>(outputLength);
	Marshal::Copy(System::IntPtr(output), result, 0, outputLength);

	delete[] input;
	delete[] output;

	return result;
}

cli::array<System::Byte> ^ SevenZCompression::SevenZCompressor::Decompress(cli::array<System::Byte> ^ data) {

	byte *input = new byte[data->Length];
	Marshal::Copy(data, 0, System::IntPtr(input), data->Length);

	unsigned int outputLength;
	byte *output = SevenZCompression::Uncompress(input, data->Length, outputLength);

	cli::array<System::Byte> ^ result = gcnew cli::array<System::Byte>(outputLength);
	Marshal::Copy(System::IntPtr(output), result, 0, outputLength);

	delete[] input;
	delete[] output;

	return result;
}
