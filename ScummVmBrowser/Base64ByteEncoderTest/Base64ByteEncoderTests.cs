using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Base64ByteEncoderTest
{
    [TestClass]
    public class Base64ByteEncoderTests
    {
        [TestMethod]
        public void CanDecodeAndEncode()
        {
			byte[] TestData = new byte[] { 12, 33, 44 };

			Base64ByteEncoder.Base64ByteEncoder e = new Base64ByteEncoder.Base64ByteEncoder();

			Assert.IsTrue(TestData.SequenceEqual(e.ByteDecode(e.ByteEncode(TestData))));
        }
    }
}
