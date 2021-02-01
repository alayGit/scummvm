
self.onmessage = function (e) {
	importScripts("/scripts/pako_inflate.min.js");
	importScripts("/scripts/yEncoding.js");

	var port = e.data.workerPort;

	self.onmessage = function (e) {

		let dataWithUncompressedBuffers = [];
		e.data.frames.forEach(
			f => {
				f.UncompressedPictureBuffer = decompress(f.CompressedBuffer);
				f.CompressedBuffer = undefined;

				if (f.CompressedPaletteBuffer) {
					f.UncompressedPaletteBuffer = convertPaletteByteArrayToPaletteDictionary(decompress(f.CompressedPaletteBuffer));
				}

				f.CompressedPaletteBuffer = undefined;
				dataWithUncompressedBuffers.push(f);
			});

		port.postMessage(dataWithUncompressedBuffers);
	};
}

function convertPaletteByteArrayToPaletteDictionary(paletteString) {
	const NO_COLORS = 256;
	const NO_BYTES_PER_PIXEL = 4;
	const MAX_VALUE_BYTE = 255;
	const MIN_VALUE_BYTE = 0;
	let paletteNo = 0;
	let digit = "";
	let colourComponent = 0;

	let result = [];

	if (paletteString.length % (3 * NO_BYTES_PER_PIXEL) != 0) {
		throw new Exception("String is the wrong length");
	}

	paletteString.forEach(charCode => {
		let c = String.fromCharCode(charCode);

		if (digit.length < 3) {
			digit += c;
		}
		else {
			var iDigit = parseInt(digit);

			if (iDigit < MIN_VALUE_BYTE || iDigit > MAX_VALUE_BYTE) {
				throw "Fail out of range digit";
			}

			if (!result[paletteNo]) {
				result[paletteNo] = [];
			}

			result[paletteNo][colourComponent] = parseInt(digit);

			colourComponent = (colourComponent + 1) % NO_BYTES_PER_PIXEL;

			if (colourComponent == 0) {
				paletteNo = (paletteNo + 1) % NO_COLORS;
			}

			digit = c;
		}
	}
	);

	return result;
}

function decompress(data) {
	var deencoded = DecodeYEncode(data);
	var compressedUInt8 = Uint8Array.from(deencoded);

	return pako.inflate(compressedUInt8);
}


