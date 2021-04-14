using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScummToManagedMarshallingTests;
using System.Linq;

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

		[TestMethod]
		public void CanConvertBetweenUnmanagedVectorAndManagedArray()
		{
			int[] ExpectedResult = new int[] { 1, 2, 33, 66, 77 };
			Assert.IsTrue(ExpectedResult.SequenceEqual(ConverterTests.CanConvertBetweenVectorAndManagedArray(ExpectedResult)));
		}
	}
}
