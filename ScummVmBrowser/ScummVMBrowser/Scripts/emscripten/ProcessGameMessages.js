importScripts("EmscriptenProcessGameMessages.js")

var decompressModule;
var moduleReady = Module().then(m=> decompressModule = m);

processGameMessages = async deflatedAndEncodedMessageString => {
	await moduleReady;

	var decompress = decompressModule.cwrap("InflateAndDecodeGameMessage", "number", "[string, number]");

	var lengthPointer = decompressModule._malloc(4);
	var deflatedAndEncodedMessageBytes = putStringBytesIntoMemory(deflatedAndEncodedMessageString);
	var inflatedAndDecodedMessasgeBytes = decompress(deflatedAndEncodedMessageBytes, deflatedAndEncodedMessageString.length, lengthPointer);

	var result = getStringBytesFromMemory(inflatedAndDecodedMessasgeBytes, decompressModule.getValue(lengthPointer, "i32"));

	decompressModule._free(deflatedAndEncodedMessageBytes);
	decompressModule._free(lengthPointer);

	return result;
}

putStringBytesIntoMemory = string => {
	var stringBytesPointer = decompressModule._malloc(string.length);

	for (var i = 0; i < string.length; i++)
	{
		decompressModule.setValue(stringBytesPointer + i, string.charCodeAt(i), "i8");
	}

	return stringBytesPointer;
}

getStringBytesFromMemory = (stringBytesPointer, length) => {
	var string = '';

	for (var i = 0; i < length; i++) {
		string = string + String.fromCharCode(decompressModule.getValue(stringBytesPointer + i, "i8"));
	}

	return string;
}
