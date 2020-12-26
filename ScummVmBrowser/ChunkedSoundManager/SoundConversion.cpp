#pragma once
#include "SoundConversion.h"


SoundManagement::SoundConverter::SoundConverter() {
}

void SoundManagement::SoundConverter::Init(SoundOptions soundOptions, f_SoundOperated soundConverted) {
	_soundOptions = soundOptions;
	_soundConverted = soundConverted;

	int start = BASS_Init(0, _soundOptions.sampleRate, 0, 0, NULL);
	_user = nullptr;
}

SoundManagement::SoundConverter::~SoundConverter()
{
	BASS_Free();
}

void CALLBACK GetEncodedDataClb(HENCODE handle, DWORD channel, const void* buffer, DWORD length, QWORD offset, void* user)
{
	((SoundManagement::SoundConverter*)user)->GetEncodedData(handle, channel, buffer, length);
}

void SoundManagement::SoundConverter::ConvertPcmToFlac(byte* pcm, int length, void* user)
{
	_user = user;

	const int BufferSize = 256;
	HSTREAM stream = BASS_StreamCreate(_soundOptions.sampleRate, NO_CHANNELS, BASS_STREAM_DECODE, STREAMPROC_PUSH, nullptr);
	bool successfullyPushedData = BASS_StreamPutData(stream, pcm, length) != -1;

	if (!successfullyPushedData)
	{
		throw "Error did not push data. The error code was " + BASS_ErrorGetCode();
	}

	HENCODE encoder = BASS_Encode_FLAC_Start(stream, "--best  --no-padding", BASS_ENCODE_AUTOFREE | BASS_ENCODE_LIMIT, &GetEncodedDataClb, this); // set a WAV writer on it

	int result = 1;
	while (result) { // processing loop...
		byte* buffer = new byte[256];

		result = BASS_ChannelGetData(stream, buffer, BufferSize); // process the decoder until it fails/ends

		delete[] buffer;
	}

	BASS_StreamFree(stream); // free the decoder (and WAV writer due to AUTOFREE)
}

void SoundManagement::SoundConverter::GetEncodedData(HENCODE handle, DWORD channel, const void* buffer, DWORD length)
{
	DWORD encoderStatus = BASS_Encode_IsActive(handle);

	_workingBuffer.resize(_workingBuffer.size() + length);
	memcpy(&_workingBuffer[_workingBuffer.size() - length], buffer, length);

	if (encoderStatus == BASS_ACTIVE_STOPPED)
	{
		byte* completeSound = new byte[_workingBuffer.size()];
		memcpy(completeSound, &_workingBuffer[0], _workingBuffer.size());
		_soundConverted(completeSound, _workingBuffer.size(), _user);
		_workingBuffer.resize(0);
	}

}

void SoundManagement::SoundConverter::ProcessSound(byte *pcm, int length, void *user) {
	ConvertPcmToFlac(pcm, length, user);
}
