var _screenBufferMap = new Map();
var _addOrder = new Array();

self.onmessage = function (postedMessage) {
	importScripts("/scripts/pako.min.js");
	importScripts("/scripts/yEncoding.js");

	postedMessage.data.fromGameMessageMessageWorkerToPictureWorker.onmessage = function (toOffScreenCanvasWorkerMessage) {
		var getPictureBuffer = (frameSetPart) => {
			if (_screenBufferMap[frameSetPart.ScreenBufferHash] == undefined) {
				_screenBufferMap[frameSetPart.ScreenBufferHash] = decode(frameSetPart.PictureBuffer);
				_addOrder.push(frameSetPart.ScreenBufferHash);
			}

			var result = _screenBufferMap[frameSetPart.ScreenBufferHash];

			if (_addOrder.length > 500) {
				_screenBufferMap.delete(_addOrder.pop());
			}

			return result;
		}

		toOffScreenCanvasWorkerMessage.data.frameSets.forEach(
			frameSet => {
				let dataWithUncompressedBuffers = [];
				frameSet.forEach(
					frameSetPart => {
						frameSetPart.PictureBuffer = getPictureBuffer(frameSetPart);
			
						if (frameSetPart.PaletteBuffer) {
							frameSetPart.PaletteBuffer = convertPaletteByteArrayToPaletteDictionary(decode(frameSetPart.PaletteBuffer));
						}

						frameSetPart.CompressedPaletteBuffer = undefined;
						dataWithUncompressedBuffers.push(frameSetPart);
					});
				postedMessage.data.toOffScreenCanvasWorker.postMessage(dataWithUncompressedBuffers);
			}
		)

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


function decode(data) {
	return DecodeYEncode(data);
}


