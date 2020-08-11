#include "pch.h"
#include "yEncDotNet.h"
yEncDotNet::YEncDotNet::YEncDotNet(ILogger^ logger, LoggingCategory category)
{
    _logger = logger;
    _category = category;
}
System::String^ yEncDotNet::YEncDotNet::AssciiEncode(cli::array<System::Byte>^ input)
{
    yEnc::Encoder encoder;
    Byte* inputBuffer = nullptr;
    Encoding^ encoding = Encoding::GetEncoding("iso-8859-1");

    try
    {
        if (input->Length == 0)
        {
            return System::String::Empty;
        }

        inputBuffer = new Byte[input->Length];
        Marshal::Copy(input, 0, System::IntPtr(inputBuffer), input->Length);

        std::vector<Byte> outputBuffer;

        uInt col = 0;

        int resultSize = encoder.encode_buffer(inputBuffer, outputBuffer, input->Length, &GetEmptyCrc(), &col);

        return encoding->GetString(&outputBuffer[0], resultSize);
    }
    catch (...)
    {
        _logger->LogMessage(LoggingLevel::Error, _category, ErrorMessage::YEncodeFailure, encoding->GetString(input));

        throw gcnew UnmanagedException("YEncode error");
    }
    finally
    {
        if (inputBuffer != nullptr)
        {
            delete[] inputBuffer;
        }
    }
}

cli::array<System::Byte>^ yEncDotNet::YEncDotNet::AssciiDecode(System::String^ input)
{
    yEnc::Encoder encoder;
    Byte* inputBuffer = nullptr;

    try
    {
        inputBuffer = new Byte[input->Length];
        std::vector<Byte> outputBuffer;
        
        cli::array<Byte>^ charArray = gcnew cli::array<Byte>(input->Length);

        int counter = 0;
        for each (wchar_t c in input->ToCharArray())
        {
            charArray[counter] = (Byte)c;

            counter++;
        }

        Marshal::Copy(charArray, 0, System::IntPtr(inputBuffer), input->Length);

        bool escape = false;
        int resultSize = encoder.decode_buffer(inputBuffer, outputBuffer, input->Length, &GetEmptyCrc(), &escape);

        cli::array<System::Byte>^ result = gcnew cli::array<System::Byte>(resultSize);

        Marshal::Copy(System::IntPtr(&outputBuffer[0]), result, 0, result->Length);

        return result;
    }
    catch (...)
    {
        _logger->LogMessage(LoggingLevel::Error, _category, ErrorMessage::YDecodeFailure, input);

        throw gcnew UnmanagedException("YEncode error");
    }
    finally
    {
        if (inputBuffer != nullptr)
        {
            delete[] inputBuffer;
        }
    }
}

Crc32 yEncDotNet::YEncDotNet::GetEmptyCrc()
{
    Crc32 crc = Crc32();
    crc.bytes = 0;
    crc.crc = 0;

    return crc;
}
