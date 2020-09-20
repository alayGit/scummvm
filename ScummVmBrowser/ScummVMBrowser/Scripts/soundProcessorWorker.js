
self.onmessage = function (e) {
	importScripts("/scripts/pako_inflate.min.js");
	var deencoded = DecodeYEncode(e.data);
	var compressedUInt8 = Uint8Array.from(deencoded);

	postMessage(pako.inflate(compressedUInt8));
};


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

