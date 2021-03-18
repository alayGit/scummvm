call "C:\emsdk\emsdk\emsdk_env.bat"
call em++  ^
Emscripten7ZCompression.cpp ^
..\7ZCompression\7ZCompression.cpp ^
..\..\ExternalLibraries\source\7z\LzmaLib.c ^
..\..\ExternalLibraries\source\7z\Alloc.c ^
..\..\ExternalLibraries\source\7z\LzmaDec.c ^
..\..\ExternalLibraries\source\7z\LzmaEnc.c ^
..\..\ExternalLibraries\source\7z\LzFind.c ^
-o Emscripten7ZCompression.html -s LLD_REPORT_UNDEFINED -s ALLOW_MEMORY_GROWTH=1 -v --no-entry -s "EXTRA_EXPORTED_RUNTIME_METHODS=['getValue', 'setValue', 'cwrap', 'printErr']"

call copy Emscripten7ZCompression.wasm C:\scumm\ScummVmBrowser\Emscripten7ZCompression\..\ScummVMBrowser\Scripts
call copy Emscripten7ZCompression.js C:\scumm\ScummVmBrowser\Emscripten7ZCompression\..\ScummVMBrowser\Scripts




