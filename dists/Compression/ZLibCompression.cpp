#include "pch.h"

#include "ZLibCompression.h"

cli::array<System::Byte>^ Compression::ZLibCompression::Compress(cli::array<System::Byte>^ input)
{
	Byte* inputBuffer = nullptr;
	Byte* outputBuffer = nullptr;
	try
	{
		inputBuffer = new Byte[input->Length];


		Marshal::Copy(input, 0, (System::IntPtr) inputBuffer, input->Length);
		

		// STEP 1.
		// deflate a into b. (that is, compress a into b)

		// zlib struct
		z_stream defstream;
		defstream.zalloc = Z_NULL;
		defstream.zfree = Z_NULL;
		defstream.opaque = Z_NULL;
		// setup "a" as the input and "b" as the compressed output
		defstream.avail_in = (uInt)input->Length;
		defstream.next_in = (Bytef*)inputBuffer; // input char array

		// the actual compression work.
		deflateInit(&defstream, Z_BEST_COMPRESSION);

		int deflateBounds = deflateBound(&defstream, input->Length);
		outputBuffer = new Byte[deflateBounds];
		defstream.next_out = (Bytef*)outputBuffer; // output char array
		defstream.avail_out = deflateBounds;

		deflate(&defstream, Z_FINISH);
		deflateEnd(&defstream);

		cli::array<System::Byte>^ output = gcnew cli::array<System::Byte>(defstream.total_out);

		Marshal::Copy((System::IntPtr)outputBuffer, output, 0, output->Length);

		return output;
	}
	finally {
		if (inputBuffer != nullptr)
		{
			delete[] inputBuffer;
		}

		if (outputBuffer != nullptr)
		{
			delete outputBuffer;
		}
	}
}

cli::array<System::Byte>^ Compression::ZLibCompression::Decompress(cli::array<System::Byte>^ input)
{
	Byte* inputBuffer = nullptr;
	Byte* outputBuffer = nullptr;
	try
	{
		inputBuffer = new Byte[input->Length];
		Marshal::Copy(input, 0, (System::IntPtr) inputBuffer, input->Length);
		int outputBufferBounds = compressBound(input->Length);
		outputBuffer = new Byte[outputBufferBounds];

		z_stream infstream;
		infstream.zalloc = Z_NULL;
		infstream.zfree = Z_NULL;
		infstream.opaque = Z_NULL;
		// setup "b" as the input and "c" as the compressed output
		infstream.avail_in = (uInt)input->Length + 1; // size of input
		infstream.next_in = (Bytef*)inputBuffer; // input char array
		infstream.avail_out = (uInt)input->Length * 1000; // size of output
		infstream.next_out = (Bytef*)outputBuffer; // output char array
		// the actual DE-compression work.

		inflateInit(&infstream);
		inflate(&infstream, Z_NO_FLUSH);
		inflateEnd(&infstream);

		int z = strlen((const char*)outputBuffer);
		cli::array<System::Byte>^ output = gcnew cli::array<System::Byte>(infstream.total_out);

		Marshal::Copy((System::IntPtr)outputBuffer, output, 0, infstream.total_out);

		return output;
	}
	finally {
		if (inputBuffer != nullptr)
		{
			delete[] inputBuffer;
		}

		if (outputBuffer != nullptr)
		{
			delete outputBuffer;
		}
	}
}
