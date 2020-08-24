using System;
using System.Linq;
using Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompressionTests
{
    [TestClass]
    public class CompressionTests
    {
        [TestMethod]
        public void ZLibCompressAndDecompress()
        {
			byte[] input = new byte[] { 8, 45, 56, 77, 88, 99 };
			ZLibCompression zLibCompression = new ZLibCompression();

			Assert.IsTrue(input.SequenceEqual(zLibCompression.Decompress(zLibCompression.Compress(input))));
        }
    }
}
