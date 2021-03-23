var fromGameMessageMessageWorkerToPictureWorker;
var fromGameMessageMessageWorkerToSoundWorker;
var decompressionModule;

importScripts("/scripts/emscripten/ProcessGameMessages.js");

self.onmessage = e => {
	importScripts("/scripts/pako.min.js");
	importScripts("/scripts/yEncoding.js");
	
	if (e.data.hasOwnProperty('fromGameMessageMessageWorkerToPictureWorker')) {
		fromGameMessageMessageWorkerToPictureWorker = e.data.fromGameMessageMessageWorkerToPictureWorker;
	}
	else if (e.data.hasOwnProperty('toGameWorkerChannel') && e.data.hasOwnProperty('fromGameMessageMessageWorkerToSoundWorker')) {
		fromGameMessageMessageWorkerToSoundWorker = e.data.fromGameMessageMessageWorkerToSoundWorker;
		e.data.toGameWorkerChannel.onmessage = async e => {
		
			var inflated = await processGameMessages(e.data);
			var messages = JSON.parse(inflated);

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




