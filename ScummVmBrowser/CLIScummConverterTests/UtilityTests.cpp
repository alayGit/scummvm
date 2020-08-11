#include "UtilityTests.h";

void CLIScummConverterTests::UtilityTests::ManagedStringToCommonStringCorrectConverts()
{
	String^ testString = "Hello World";

	Common::String commonString = Utilities::Converters::ManagedStringToCommonString(testString);

	String^ result = gcnew String(commonString.c_str());

	Assert::AreEqual(testString, result);
}
