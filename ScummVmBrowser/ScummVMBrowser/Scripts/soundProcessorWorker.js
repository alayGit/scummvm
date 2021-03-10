
self.onmessage = function (e) {
	importScripts("/scripts/pako.min.js");
	importScripts("/scripts/yEncoding.js");

	e.data.fromGameMessageMessageWorkerToSoundWorker.onmessage = function (e) {
		e.data.soundSamples.forEach(
			s => {
				var decoded = DecodeYEncode(s);
				var decodedUInt8 = Uint8Array.from(decoded);

				postMessage(Array.from(decodedUInt8));
			}
		);
	}
};

