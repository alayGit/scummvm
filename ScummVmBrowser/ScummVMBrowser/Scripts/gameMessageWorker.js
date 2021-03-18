var fromGameMessageMessageWorkerToPictureWorker;
var fromGameMessageMessageWorkerToSoundWorker;
var decompressionModule;

self.onmessage = function (e) {
	importScripts("/scripts/pako.min.js");
	importScripts("/scripts/yEncoding.js");
	
	if (e.data.hasOwnProperty('fromGameMessageMessageWorkerToPictureWorker')) {
		fromGameMessageMessageWorkerToPictureWorker = e.data.fromGameMessageMessageWorkerToPictureWorker;
	}
	else if (e.data.hasOwnProperty('toGameWorkerChannel') && e.data.hasOwnProperty('fromGameMessageMessageWorkerToSoundWorker')) {
		fromGameMessageMessageWorkerToSoundWorker = e.data.fromGameMessageMessageWorkerToSoundWorker;
		e.data.toGameWorkerChannel.onmessage = function (e) {
		
			var inflated = decode(e.data);
			var messages = JSON.parse(stringFromUTF8Array(inflated));

			messages.forEach(function (item) {
				switch (item.Key) {
					case 0: //Audio 
						fromGameMessageMessageWorkerToSoundWorker.postMessage({ soundSamples: item.Value } );
						break;
					case 1: //Frames
						fromGameMessageMessageWorkerToPictureWorker.postMessage({ frameSets: item.Value });
						break;
				}
			});
		};
	}
	else {
		throw Error("Failed unknown data object");
	}
}


function stringFromUTF8Array(data) {
	var result = "";

	for (var i = 0; i < data.length; i++) {
		result += String.fromCharCode(data[i]);
	}

	return result;
}

function decode(data) {
	var deencoded = DecodeYEncode(data);
	var compressedUInt8 = Uint8Array.from(deencoded);

	return pako.inflate(compressedUInt8);
}


