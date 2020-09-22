
self.onmessage = function (e) {
	console.log("In worker data processor");
	importScripts("/scripts/pako_inflate.min.js");
	importScripts("/scripts/yEncoding.js");

	var port = e.data.workerPort;

	self.onmessage = function (e) {

		let dataWithUncompressedBuffers = [];
		e.data.frames.forEach(
			f => {
				var deencoded = DecodeYEncode(f.Buffer);
				var compressedUInt8 = Uint8Array.from(deencoded);

				f.Buffer = pako.inflate(compressedUInt8);

				dataWithUncompressedBuffers.push(f);
			});

		port.postMessage(dataWithUncompressedBuffers);
	};
}


