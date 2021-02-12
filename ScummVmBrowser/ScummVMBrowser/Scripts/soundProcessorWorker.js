
self.onmessage = function (e) {
	importScripts("/scripts/pako_inflate.min.js");
	importScripts("/scripts/yEncoding.js");

	var soundSamples = JSON.parse(e.data);

	soundSamples.forEach(
		s => {
			var deencoded = DecodeYEncode(s);
			var compressedUInt8 = Uint8Array.from(deencoded);

			postMessage(Array.from(pako.inflate(compressedUInt8)));
		}
	);
};

