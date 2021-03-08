using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevenZip;
using System;
using System.Linq;

namespace SevenZipCompressionTests
{
    [TestClass]
    public class SevenZipCompressionTests
    {
		[TestMethod]
		public void CanCompressAndDecompress()
        {
			SevenZipCompression sevenZipCompression = new SevenZipCompression();

			byte[] testData = new byte[] { 12, 34, 55, 12, 45, 77, 88, 12, 34, 55, 12, 45, 77, 88, 12, 34, 55, 12, 45, 77, 82, 34, 55, 12, 45, 77, 12, 34, 55, 12, 45, 77, 82, 34, 55, 12, 45, 77, 12, 34, 55, 12, 45, 77, 88 };

			Assert.IsTrue(testData.SequenceEqual(sevenZipCompression.Decompress(sevenZipCompression.Compress(testData))));
		}
    }
}
