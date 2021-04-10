#include "pch.h"

#include "ConverterTests.h"

bool ScummToManagedMarshallingTests::ConverterTests::CanConvertBetweenManagedAndCommonString() {
	System::String ^ testString = "fish";
	return Converters::CommonStringToManagedString(&Converters::ManagedStringToCommonString(testString)) == testString;
}
