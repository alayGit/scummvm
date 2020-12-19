
self.onmessage = function (e) {
	importScripts("/scripts/pako_inflate.min.js");
	importScripts("/scripts/yEncoding.js");
	importScripts("/scripts/aurora/aurora.js");
	importScripts("/scripts/aurora/decoders/aac.js");
	var deencoded = DecodeYEncode(e.data);
	var compressedUInt8 = Uint8Array.from(deencoded);

	postMessage(Array.from(pako.inflate(compressedUInt8)));
};

