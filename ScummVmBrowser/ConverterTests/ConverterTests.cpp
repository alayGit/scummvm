#include "ConverterTests.h"

bool ScummToManagedMarshallingTests::ConverterTests::CanConvertBetweenManagedAndCommonString() {
	System::String ^ testString = "fish";
	return Converters::CommonStringToManagedString(&Converters::ManagedStringToCommonString(testString)) == testString;
}

cli::array<int> ^ ScummToManagedMarshallingTests::ConverterTests::CanConvertBetweenVectorAndManagedArray(cli::array<int> ^ array) {
	std::vector<int> *convertToUnmanaged = Converters::MarshalManagedArrayToVector<int>(array);

	cli::array<int> ^ convertToManaged = Converters::MarshalVectorToManagedArray<int>(convertToUnmanaged);

	return convertToManaged;
} 
