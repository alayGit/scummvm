
self.onmessage = function (e) {
	importScripts("/scripts/pako.min.js");

	postMessage(Array.from(pako.deflate(e.data)));
};
