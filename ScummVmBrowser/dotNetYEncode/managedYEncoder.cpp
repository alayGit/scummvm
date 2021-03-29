#include "pch.h"
#include "managedYEncoder.h"

ManagedYEncoder::ManagedYEncoder::ManagedYEncoder(ManagedCommon::Interfaces::ILogger ^ logger, ManagedCommon::Enums::Logging::LoggingCategory category) {
	_logger = logger;
	_category = category;
}

System::Text::Encoding ^ ManagedYEncoder::ManagedYEncoder::TextEncoding::get() {
	return System::Text::Encoding::GetEncoding("iso-8859-1");
}

System::String^ ManagedYEncoder::ManagedYEncoder::ByteEncode(cli::array<System::Byte>^ input)
{
    yEnc::Encoder encoder;
    Byte* inputBuffer = nullptr;
	Byte *outputBuffer = nullptr;

    try
    {
        if (input->Length == 0)
        {
            return System::String::Empty;
        }

        inputBuffer = new Byte[input->Length];
        Marshal::Copy(input, 0, System::IntPtr(inputBuffer), input->Length);

        uInt col = 0;

		uInt resultSize;
        outputBuffer = encoder.encode_buffer(inputBuffer, input->Length, resultSize);

        return TextEncoding->GetString(&outputBuffer[0], resultSize);
    }
    catch (...)
    {
        throw gcnew System::Exception("YEncode error");
    }
    finally
    {
        if (inputBuffer != nullptr)
        {
            delete[] inputBuffer;
        }

		 if (outputBuffer != nullptr) {
			delete[] outputBuffer;
		}
    }
}

cli::array<System::Byte> ^ ManagedYEncoder::ManagedYEncoder::ByteDecode(System::String ^ input) {
	yEnc::Encoder encoder;
	Byte *inputBuffer = nullptr;

	try {
		inputBuffer = new Byte[input->Length];
		Byte* outputBuffer;

		cli::array<Byte> ^ charArray = gcnew cli::array<Byte>(input->Length);

		int counter = 0;
		for each(wchar_t c in input->ToCharArray()) {
				charArray[counter] = (Byte)c;

				counter++;
			}

		Marshal::Copy(charArray, 0, System::IntPtr(inputBuffer), input->Length);

		bool escape = false;
		uInt resultSize;
		outputBuffer = encoder.decode_buffer(inputBuffer, input->Length, resultSize);

		cli::array<System::Byte> ^ result = gcnew cli::array<System::Byte>(resultSize);

		Marshal::Copy(System::IntPtr(&outputBuffer[0]), result, 0, result->Length);

		return result;
	} catch (...) {
		_logger->LogMessage(LoggingLevel::Error, _category, ErrorMessage::YDecodeFailure, input);

		throw gcnew UnmanagedException("YEncode error");
	} finally {
		if (inputBuffer != nullptr) {
			delete[] inputBuffer;
		}
	}
}

Crc32 ManagedYEncoder::ManagedYEncoder::GetEmptyCrc()
{
    Crc32 crc = Crc32();
    crc.bytes = 0;
    crc.crc = 0;

    return crc;
}
