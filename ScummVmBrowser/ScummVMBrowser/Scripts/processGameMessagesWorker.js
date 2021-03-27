importScripts("EmscriptenProcessGameMessages.js")
var decompressModule;
var moduleReady = Module().then(m=> decompressModule = m);

self.onmessage = async e => {
	await moduleReady;

	var decompress = decompressModule.cwrap("InflateAndDecodeGameMessage", "number", "[string, number]");

	var lengthPointer = decompressModule._malloc(4);
	var deflatedAndEncodedMessageBytes = putStringBytesIntoMemory(e.data);
	var inflatedAndDecodedMessasgeBytes = decompress(deflatedAndEncodedMessageBytes, e.data.length, lengthPointer);

	var result = getStringBytesFromMemory(inflatedAndDecodedMessasgeBytes, decompressModule.getValue(lengthPointer, "i32"));

	decompressModule._free(deflatedAndEncodedMessageBytes);
	decompressModule._free(lengthPointer);

	self.postMessage(result);
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

	var uInt8Array = new Uint8Array(decompressModule.HEAPU8.buffer, stringBytesPointer, length).forEach(
		b => {
			string = string +  String.fromCharCode(b);
		}
	);

	return string;
}
