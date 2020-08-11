#include "pch.h"
#include "yEncoder.h"

yEnc::Encoder::Encoder()
{
}

int yEnc::Encoder::encode_buffer(Byte* input_buffer, std::vector<Byte>& output_buffer, uInt bytes, Crc32* crc, uInt* col)
{
    uInt encoded = 0;
    uInt in_ind;
    uInt out_ind;
    Byte byte;

    out_ind = 0;
    for (in_ind = 0; in_ind < bytes; in_ind++) {
        byte = (Byte)(input_buffer[in_ind] + 42);
        crc_update(crc, input_buffer[in_ind]);
        switch (byte) {
        case ZERO:
        case LF:
        case CR:
        case ESC:
            goto escape_string;
        case TAB:
        case SPACE:
            if (*col == 0 || *col == LINESIZE - 1) {
                goto escape_string;
            }
        case DOT:
            if (*col == 0) {
                goto escape_string;
            }
        default:
            goto plain_string;
        }
    escape_string:
        byte = (Byte)(byte + 64);
        output_buffer.push_back(ESC);
        out_ind++;
        (*col)++;
    plain_string:
        output_buffer.push_back(byte);
        out_ind++;
        (*col)++;
        encoded++;
        if (*col >= LINESIZE) {
            output_buffer.push_back(CR);
            output_buffer.push_back(LF);
            out_ind = out_ind + 2;
            *col = 0;
        }
    }
    return out_ind;
}

int yEnc::Encoder::decode_buffer(Byte* input_buffer, std::vector<Byte>& output_buffer, uInt bytes, Crc32* crc, bool* escape)
{
    uInt read_ind;
    uInt decoded_bytes;
    Byte byte;

    decoded_bytes = 0;
    for (read_ind = 0; read_ind < bytes; read_ind++) {
        byte = input_buffer[read_ind];
        if (*escape) {
            byte = (Byte)(byte - 106);
            *escape = 0;
        }
        else if (byte == ESC) {
            *escape = 1;
            continue;
        }
        else if (byte == LF || byte == CR) {
            continue;
        }
        else {
            byte = (Byte)(byte - 42);
        }
        output_buffer.push_back(byte);
        decoded_bytes++;
        crc_update(crc, byte);
    }
    return decoded_bytes;
}

void yEnc::Encoder::crc_update(Crc32* crc, uInt c)
{
    crc->crc = crc_tab[(crc->crc ^ c) & 0xff] ^ ((crc->crc >> 8) & 0xffffff);
    crc->bytes++;
}
