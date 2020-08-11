using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLIScumm;
using ConfigStore;
using ManagedCommon.Enums;
using ManagedCommon.Implementations;
using ManagedCommon.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SignalRSelfHost;
using TestMocks;

namespace SignalRSelfHostTest
{
    [TestClass]
    public class ScummHubTests
    {
        MockWrapper _wrapper;
        MockRealTimeDataBus _mockRealTimeDataBus;
        SignalRSelfHost.CliScumm _scummHub;
        [TestInitialize]
        public void SetUp()
        {
            _mockRealTimeDataBus = new MockRealTimeDataBus();
            _wrapper = new MockWrapper();
            _scummHub = new SignalRSelfHost.CliScumm(_wrapper, new JsonConfigStore(), _mockRealTimeDataBus, null, null, new WindowsEventLogger());
        }

        [TestMethod]
        public async Task CanRunGame()
        {
            IEnumerable<ExpectedNextFrameResult> expectedNextFrameResults = new List<ExpectedNextFrameResult>()
            {
                new ExpectedNextFrameResult() {PicBuff = new byte[] {1,1,45,3,1}, H = 4, W = 3, X = 4, Y = 5},
                new ExpectedNextFrameResult() {PicBuff = new byte[] {2,6,6,7,9}, H = 74, W = 73, X = 74, Y = 75}
            };

            _mockRealTimeDataBus.NextFrame = new MockNextFrame(expectedNextFrameResults);

            await _scummHub.RunGameAsync(AvailableGames.kq3, null);

            foreach (ExpectedNextFrameResult expectedNextFrameResult in expectedNextFrameResults)
            {
                         List<ScreenBuffer> screenBuffers = new List<ScreenBuffer>() { new ScreenBuffer() { Buffer = expectedNextFrameResult.PicBuff, H = expectedNextFrameResult.H, W = expectedNextFrameResult.W, X = expectedNextFrameResult.X, Y = expectedNextFrameResult.Y } };
                    _wrapper.OnCopyRectToScreen.Invoke(screenBuffers);
            }

            Assert.IsTrue(_mockRealTimeDataBus.NextFrame.EveryFrameMatched);
            Assert.IsTrue(_mockRealTimeDataBus.NextFrame.NextFrameCalledCorrectNumberOfTimes);

            await _scummHub.EndGame();
        }

        [TestMethod]
        public void CanEnqueueControlKey()
        {
            List<ControlKeys> controlKeysList = new List<ControlKeys>() { ControlKeys.ArrowDown, ControlKeys.ArrowLeft, ControlKeys.Delete };

            foreach (ControlKeys controlKey in controlKeysList)
            {
                _scummHub.EnqueueControlKey(controlKey);
            }

            Assert.IsTrue(controlKeysList.SequenceEqual(_wrapper.ReceivedControlKeys));
        }

        [TestMethod]
        public void CanEnqueueString()
        {
            List<String> stringList = new List<String>() { "abc", "def", "ghi" };

            foreach (String str in stringList)
            {
                _scummHub.EnqueueString(str);
            }

            Assert.IsTrue(stringList.SequenceEqual(_wrapper.ReceivedStrings));
        }

        [TestMethod]
        public async Task QuitingFiresQuitEvent()
        {
            bool hasFired = false;

            _scummHub.OnQuit += () =>
            {
                hasFired = true;
                return Task.CompletedTask;
            };

            await _scummHub.RunGameAsync(AvailableGames.kq3, null);

            _scummHub.Quit(); //TODO: Fix

            Assert.IsTrue(hasFired);
        }
    }
}
