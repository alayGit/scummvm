using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScummToManagedMarshallingTests;
namespace ConverterTestersWrapper
{
    [TestClass]
    public class ScummToManagedMarshallingTestsWrapper
    {
        [TestMethod]
        public void CanConvertBetweenManagedAndCommonString()
        {
			Assert.IsTrue(ConverterTests.CanConvertBetweenManagedAndCommonString());
		}
    }
}
