using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevenZCompression;
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
			const int testDataLength = 6000;
			SevenZCompressor sevenZipCompression = new SevenZCompressor();

			byte[] testData = new byte[testDataLength];

			for(int i = 0; i < testDataLength; i++)
			{
				testData[i] = ((byte) (i % 256));
			}

			Assert.IsTrue(testData.SequenceEqual(sevenZipCompression.Decompress(sevenZipCompression.Compress(testData))));
		}
    }
}
