importScripts("EmscriptenProcessGameMessages.js")
var decompressModule;
var moduleReady = Module().then(m=> decompressModule = m);

self.onmessage = async e => {
	await moduleReady;

	var decompress = decompressModule.cwrap("InflateAndDecodeGameMessage", "number", ["number", "number", "number"]);
	var alloc = decompressModule.cwrap("alloc", "number", ["number"]);
	var dealloc = decompressModule.cwrap("dealloc", null, ["number"]);

	var lengthPointer = alloc(4);
	var deflatedAndEncodedMessageBytes = putStringBytesIntoMemory(e.data, alloc);
	var inflatedAndDecodedMessasgeBytes = decompress(deflatedAndEncodedMessageBytes, e.data.length, lengthPointer);

	var result = getStringBytesFromMemory(inflatedAndDecodedMessasgeBytes, decompressModule.getValue(lengthPointer, "i32"));

	dealloc(deflatedAndEncodedMessageBytes);
	dealloc(lengthPointer);

	self.postMessage(result);
}

putStringBytesIntoMemory = (string, alloc) => {
	var stringBytesPointer = alloc(string.length);

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
