// Emscripten7ZCompression.cpp : Defines the functions for the static library.
//

#include "EmscriptenProcessGameMessages.h"

EMSCRIPTEN_BINDINGS(my_module) {
	function("InflateAndDecodeGameMessage", &JSWasm::InflateAndDecodeGameMessage, emscripten::allow_raw_pointers());
}
