#include "pch.h"

#include "ManagedZLibCompression.h"

cli::array<System::Byte> ^ ManagedZLibCompression::ManagedZLibCompression::Compress(cli::array<System::Byte> ^ input) {
	byte *inputBuffer = nullptr;
	byte *outputBuffer = nullptr;
	try {
		inputBuffer = new byte[input->Length];

		Marshal::Copy(input, 0, (System::IntPtr)inputBuffer, input->Length);

		ZLibCompression::ZLibCompression compresser;

		int outputLength = 0;
		outputBuffer = compresser.Compress(inputBuffer, input->Length, outputLength);

		cli::array<System::Byte> ^ output = gcnew cli::array<System::Byte>(outputLength);

		Marshal::Copy((System::IntPtr)outputBuffer, output, 0, output->Length);

		return output;
	} finally {
		if (inputBuffer != nullptr) {
			delete[] inputBuffer;
		}

		if (outputBuffer != nullptr) {
			delete outputBuffer;
		}
	}
}

cli::array<System::Byte> ^ ManagedZLibCompression::ManagedZLibCompression::Decompress(cli::array<System::Byte> ^ input) {
	byte *inputBuffer = nullptr;
	byte *outputBuffer = nullptr;
	try {
		inputBuffer = new byte[input->Length];

		Marshal::Copy(input, 0, (System::IntPtr)inputBuffer, input->Length);

		ZLibCompression::ZLibCompression compresser;

		int outputLength = 0;
		outputBuffer = compresser.Decompress(inputBuffer, input->Length, outputLength);

		cli::array<System::Byte> ^ output = gcnew cli::array<System::Byte>(outputLength);

		Marshal::Copy((System::IntPtr)outputBuffer, output, 0, output->Length);

		return output;
	} finally {
		if (inputBuffer != nullptr) {
			delete[] inputBuffer;
		}

		if (outputBuffer != nullptr) {
			delete[] outputBuffer;
		}
	}
}
