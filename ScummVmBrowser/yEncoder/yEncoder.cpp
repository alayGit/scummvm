#include "pch.h"
#include "yEncoder.h"

yEnc::Encoder::Encoder()
{
}

Byte* yEnc::Encoder::encode_buffer(Byte *inputBuffer, uInt inputLength, uInt &output_bufferLength) {
    uInt encoded = 0;
    uInt in_ind;
    uInt out_ind;
    Byte byte;
	int col = 0;


	Crc32 crcEmptyCrc = getEmptyCrc();

	std::vector<Byte> outputVector = std::vector<Byte>();

    out_ind = 0;
    for (in_ind = 0; in_ind < inputLength; in_ind++) {
        byte = (Byte)(inputBuffer[in_ind] + 42);
		crc_update(&crcEmptyCrc, inputBuffer[in_ind]);
        switch (byte) {
        case ZERO:
        case LF:
        case CR:
        case ESC:
            goto escape_string;
        case TAB:
        case SPACE:
            if (col == 0 || col == LINESIZE - 1) {
                goto escape_string;
            }
        case DOT:
            if (col == 0) {
                goto escape_string;
            }
        default:
            goto plain_string;
        }
    escape_string:
        byte = (Byte)(byte + 64);
        outputVector.push_back(ESC);
        out_ind++;
        col++;
    plain_string:
        outputVector.push_back(byte);
        out_ind++;
        col++;
        encoded++;
        if (col >= LINESIZE) {
            outputVector.push_back(CR);
            outputVector.push_back(LF);
            out_ind = out_ind + 2;
            col = 0;
        }
    }

    Byte* output_buffer = new Byte[outputVector.size()];

	memcpy(output_buffer, &outputVector[0], outputVector.size());
	output_bufferLength = outputVector.size();

    return output_buffer;
}

Byte* yEnc::Encoder::decode_buffer(Byte *inputBuffer, uInt inputLength, uInt &output_bufferLength) {
    uInt read_ind;
    uInt decoded_bytes;
    Byte byte;
	bool escape = false;
	Crc32 crc = getEmptyCrc();

	std::vector<Byte> outputVector;

    decoded_bytes = 0;
    for (read_ind = 0; read_ind < inputLength; read_ind++) {
        byte = inputBuffer[read_ind];
        if (escape) {
            byte = (Byte)(byte - 106);
            escape = 0;
        }
        else if (byte == ESC) {
            escape = 1;
            continue;
        }
        else if (byte == LF || byte == CR) {
            continue;
        }
        else {
            byte = (Byte)(byte - 42);
        }
        outputVector.push_back(byte);
        decoded_bytes++;
        crc_update(&crc, byte);
    }

	Byte *outputBuffer = new Byte[outputVector.size()];

	memcpy(outputBuffer, &outputVector[0], outputVector.size());
	output_bufferLength = outputVector.size();

    return outputBuffer;
}

Crc32 yEnc::Encoder::getEmptyCrc() {
	Crc32 crc = Crc32();
	crc.bytes = 0;
	crc.crc = 0;

	return crc;
}

void yEnc::Encoder::crc_update(Crc32 *crc, uInt c) {
    crc->crc = crc_tab[(crc->crc ^ c) & 0xff] ^ ((crc->crc >> 8) & 0xffffff);
    crc->bytes++;
}
