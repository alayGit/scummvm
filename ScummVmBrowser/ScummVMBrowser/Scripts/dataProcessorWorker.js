
self.onmessage = function (e) {
	console.log("In worker data processor");
	importScripts("/scripts/pako_inflate.min.js");

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

function DecodeYEncode(source) {
	var
		output = []
		, ck = false
		, i = 0
		, c
		;
	let sourceArray = Array.from(source);
	for (i = 0; i < sourceArray.length; i++) {
		c = sourceArray[i].charCodeAt(0);
		// ignore newlines
		if (c === 13 || c === 10) { continue; }
		// if we're an "=" and we haven't been flagged, set flag
		if (c === 61 && !ck) {
			ck = true;
			continue;
		}
		if (ck) {
			ck = false;
			c = c - 64;
		}
		if (c < 42 && c >= 0) {
			output.push(c + 214);
		} else {
			output.push(c - 42);
		}
	}
	return output;
};

