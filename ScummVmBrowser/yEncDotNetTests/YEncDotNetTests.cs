using System;
using System.Text;
using CommonMocks;
using ManagedCommon.Enums.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedYEncoder;
using Moq;
using ManagedCommon.Interfaces;

namespace yEncDotNetTests
{
    [TestClass]
    public class YEncDotNetTests
    {
        [TestMethod]
        public void CanEncode()
        {
            const string TestString = "I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string I am a very long string a very very very long string";

			Mock<ILogger> logger = new Mock<ILogger>();

            ManagedYEncoder.ManagedYEncoder encoder = new ManagedYEncoder.ManagedYEncoder(logger.Object, LoggingCategory.CliScummSelfHost);
           
            string encodedResult = encoder.AssciiByteEncode(Encoding.ASCII.GetBytes(TestString));

           Assert.AreEqual(TestString, Encoding.ASCII.GetString(encoder.AssciiByteDecode(encodedResult)));
        }
    }
}
