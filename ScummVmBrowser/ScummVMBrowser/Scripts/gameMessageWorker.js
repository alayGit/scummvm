var fromGameMessageMessageWorkerToPictureWorker;
var fromGameMessageMessageWorkerToSoundWorker;
var decompressionModule;

var processGameMessagesWorker = new Worker("/scripts/processGameMessagesWorker.js");

self.onmessage = e => {
	importScripts("/scripts/pako.min.js");
	importScripts("/scripts/yEncoding.js");
	
	if (e.data.hasOwnProperty('fromGameMessageMessageWorkerToPictureWorker')) {
		fromGameMessageMessageWorkerToPictureWorker = e.data.fromGameMessageMessageWorkerToPictureWorker;
	}
	else if (e.data.hasOwnProperty('toGameWorkerChannel') && e.data.hasOwnProperty('fromGameMessageMessageWorkerToSoundWorker')) {
		fromGameMessageMessageWorkerToSoundWorker = e.data.fromGameMessageMessageWorkerToSoundWorker;
		e.data.toGameWorkerChannel.onmessage = unprocessedMessage => {
		
			processGameMessagesWorker.postMessage(unprocessedMessage.data);

			processGameMessagesWorker.onmessage = (processedMessage => {
				var messages = JSON.parse(processedMessage.data);

				messages.forEach(function (item) {
					switch (item.Key) {
						case 0: //Audio 
							fromGameMessageMessageWorkerToSoundWorker.postMessage({ soundSamples: item.Value });
							break;
						case 1: //Frames
							fromGameMessageMessageWorkerToPictureWorker.postMessage({ frameSets: item.Value });
							break;
					}
				});
			});
		};
	}
	else {
		throw Error("Failed unknown data object");
	}
}




