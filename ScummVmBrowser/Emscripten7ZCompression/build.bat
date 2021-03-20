call "C:\emsdk\emsdk\emsdk_env.bat"
call em++  ^
EmscriptenProcessGameMessages.cpp ^
..\JSWasm\ProcessGameMessages.cpp ^
..\7ZCompression\7ZCompression.cpp ^
..\yEncoder\yEncoder.cpp ^
..\..\ExternalLibraries\source\7z\LzmaLib.c ^
..\..\ExternalLibraries\source\7z\Alloc.c ^
..\..\ExternalLibraries\source\7z\LzmaDec.c ^
..\..\ExternalLibraries\source\7z\LzmaEnc.c ^
..\..\ExternalLibraries\source\7z\LzFind.c ^
--bind -o EmscriptenProcessGameMessages.html -s LLD_REPORT_UNDEFINED -s ALLOW_MEMORY_GROWTH=1 -v --no-entry -s "EXTRA_EXPORTED_RUNTIME_METHODS=['getValue', 'setValue', 'cwrap', 'printErr']"

call copy EmscriptenProcessGameMessages.wasm C:\scumm\ScummVmBrowser\Emscripten7ZCompression\..\ScummVMBrowser\Scripts
call copy EmscriptenProcessGameMessages.js C:\scumm\ScummVmBrowser\Emscripten7ZCompression\..\ScummVMBrowser\Scripts




