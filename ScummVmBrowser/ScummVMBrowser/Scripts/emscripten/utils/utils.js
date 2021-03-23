convertPointerToNumber = p => {
	return ProcessGameMessagesModule.getValue(p, "i32");
}

convertPointerToArray = (p, l, lengthIsPointer = true) => {
	var actualLength = l;

	if (lengthIsPointer) {
		actualLength = convertPointerToNumber(l);
	}

	return Array.from(new Uint8Array(ProcessGameMessagesModule.HEAPU8.buffer, p, actualLength));
}

convertNumberToPointer = n => {
	p = ProcessGameMessagesModule._malloc(4);
	ProcessGameMessagesModule.setValue(p, n, "i32");

	return p;
}

convertArrayToPointer = a => {
	var array_ptr = ProcessGameMessagesModule._malloc(a.length);
	ProcessGameMessagesModule.HEAPU8.set(testData, input_ptr);

	return array_ptr;
}
