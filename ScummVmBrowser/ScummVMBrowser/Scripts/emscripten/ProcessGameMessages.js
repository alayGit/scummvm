importScripts("./emscripten/EmscriptenProcessGameMessages.js")
importScripts("./emscripten/utils/utils.js")

processGameMessages = a => {
	var gameMessagesPointer = convertArrayToPointer(a);

	var result = Module.InflateAndDecodeGameMessage(gameMessagesPointer, a.length);

	Module._free(gameMessagesPointer);

	return result;
}
