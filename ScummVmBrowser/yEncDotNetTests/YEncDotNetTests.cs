using System;
using System.Text;
using CommonMocks;
using ManagedCommon.Enums.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedYEncoder;

namespace yEncDotNetTests
{
    [TestClass]
    public class YEncDotNetTests
    {
        [TestMethod]
        public void CanEncode()
        {
            const string TestString = "I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string";

            ManagedYEncoder.ManagedYEncoder encoder = new ManagedYEncoder.ManagedYEncoder();
           
            string encodedResult = encoder.AssciiByteEncode(Encoding.ASCII.GetBytes(TestString));

           Assert.AreEqual(TestString, Encoding.ASCII.GetString(encoder.AssciiByteDecode(encodedResult)));
        }
    }
}
