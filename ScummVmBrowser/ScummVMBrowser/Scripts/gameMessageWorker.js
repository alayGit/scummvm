var fromGameMessageMessageWorkerToPictureWorker;

self.onmessage = function (e) {
	importScripts("/scripts/pako.min.js");
	importScripts("/scripts/yEncoding.js");

	var toGameWorkerChannel;

	if (e.data.hasOwnProperty('fromGameMessageMessageWorkerToPictureWorker')) {
		fromGameMessageMessageWorkerToPictureWorker = e.data.fromGameMessageMessageWorkerToPictureWorker;
	}
	else if (e.data.hasOwnProperty('toGameWorkerChannel')) {

		e.data.toGameWorkerChannel.onmessage = function (e) {

			var inflated = decompress(e.data);
			var messages = JSON.parse(stringFromUTF8Array(inflated));

			messages.forEach(function (item) {
				switch (item.Key) {
					case 0: //Audio 
						//soundWorker.postMessage(item.Value);
						break;
					case 1: //Frames
						fromGameMessageMessageWorkerToPictureWorker.postMessage({ frameSets: item.Value });
						break;
				}
			});
		};
	}
}

//Thanks to https://weblog.rogueamoeba.com/2017/02/27/javascript-correctly-converting-a-byte-array-to-a-utf-8-string/
function stringFromUTF8Array(data) {
	const extraByteMap = [1, 1, 1, 1, 2, 2, 3, 0];
	var count = data.length;
	var str = "";

	for (var index = 0; index < count;) {
		var ch = data[index++];
		if (ch & 0x80) {
			var extra = extraByteMap[(ch >> 3) & 0x07];
			if (!(ch & 0x40) || !extra || ((index + extra) > count))
				return null;

			ch = ch & (0x3F >> extra);
			for (; extra > 0; extra -= 1) {
				var chx = data[index++];
				if ((chx & 0xC0) != 0x80)
					return null;

				ch = (ch << 6) | (chx & 0x3F);
			}
		}

		str += String.fromCharCode(ch);
	}

	return str;
}

function decompress(data) {
	var deencoded = DecodeYEncode(data);
	var compressedUInt8 = Uint8Array.from(deencoded);

	return pako.inflate(compressedUInt8);
}


