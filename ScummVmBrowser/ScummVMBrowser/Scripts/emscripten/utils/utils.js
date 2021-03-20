convertPointerToNumber = p => {
	return Module.getValue(p, "i32");
}

convertPointerToArray = (p, l, lengthIsPointer = true) => {
	var actualLength = l;

	if (lengthIsPointer) {
		actualLength = convertPointerToNumber(l);
	}

	return Array.from(new Uint8Array(Module.HEAPU8.buffer, p, actualLength));
}

convertNumberToPointer = n => {
	p = Module._malloc(4);
	Module.setValue(p, n, "i32");

	return p;
}

convertArrayToPointer = a => {
	var array_ptr = Module._malloc(a.length);
	Module.HEAPU8.set(testData, input_ptr);

	return array_ptr;
}
