using System;
using System.Text;
using CommonMocks;
using ManagedCommon.Enums.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using yEncDotNet;

namespace yEncDotNetTests
{
    [TestClass]
    public class YEncDotNetTests
    {
        [TestMethod]
        public void CanEncode()
        {
            const string TestString = "I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string";

            YEncDotNet encoder = new YEncDotNet(new DoNothingLogger(), LoggingCategory.ScummVmWebBrowser);
           
            string encodedResult = encoder.AssciiEncode(Encoding.ASCII.GetBytes(TestString));

           Assert.AreEqual(TestString, Encoding.ASCII.GetString(encoder.AssciiDecode(encodedResult)));
        }
    }
}
