using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetScummTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ScummVMBrowserSelenium
{
    [TestClass]
    public class SeleniumFrameCaptureTests : FrameCaptureTests
    {
        Task _pollingTask;
        IWebDriver _driver;

        const string LocalStorageKey = "kq3";

        [TestInitialize]
        public void TestInitialize()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--disable-web-security");
            _driver = new ChromeDriver();
           
            _driver.Url = "http://localhost:44365/gameScreen/kq3/";
            _driver.Navigate();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _driver.Quit();
        }

        [TestMethod]
        public async Task CanSendControlKey()
        {
            Cropping = new Rectangle(0,0,105,43);
            string frameName = "CanSendLeft";
            int noFrames = 90;

            SetupPollingTask(frameName, noFrames);
          
            await WaitForFrame(5);

            _driver.SendKeyToCanvas(Keys.Enter);

            await WaitAdditionalFrames(5);

            _driver.SendKeyToCanvas(Keys.Escape);

            await WaitForExpectedFrameAndQuit(frameName, noFrames, _pollingTask);
        }

        [TestMethod]
        public async Task CanSendString()
        {
            string frameName = "CanSendString";
            int noFrames = 25;

            SetupPollingTask(frameName, noFrames);

            await WaitForFrame(10);

            _driver.SendKeyToCanvas(Keys.Enter);

            await WaitAdditionalFrames(5);

            _driver.SendKeyToCanvas("AbcdEFg");
            _driver.SendKeyToCanvas(Keys.Enter);

            await WaitForExpectedFrameAndQuit(frameName, noFrames, _pollingTask);
        }

        [TestMethod]
        public async Task WholePageIsRedrawnOnRefresh()
        {
            Cropping = new Rectangle(0, 0, 105, 43);
            string frameName = "CanSendLeft";
            int noFrames = 90;

            SetupPollingTask(frameName, noFrames);

            await WaitForFrame(5);

            _driver.SendKeyToCanvas(Keys.Enter);

            await WaitAdditionalFrames(5);

            _driver.SendKeyToCanvas(Keys.Escape);

            _driver.Navigate().Refresh();

            ClearFrames();

            await WaitForExpectedFrameAndQuit(frameName, noFrames, _pollingTask);
        }

        [TestMethod]
        [Ignore]
        public async Task LoadsFromLocalStorage()
        {
            Cropping = new Rectangle(0, 10, 100, 30);

            _driver.ClearLocalStorage(LocalStorageKey);
            _driver.AddToLocalStorage(LocalStorageKey, GetJsonData());

            _driver.Navigate().Refresh();

            string frameName = "CanRestore";
            int noFrames = 90;

            SetupPollingTask(frameName, noFrames);

            await WaitAdditionalFrames(5);

            _driver.SendKeyToCanvas(Keys.Enter);

            await WaitAdditionalFrames(5);

            await Restore();

            await WaitForExpectedFrameAndQuit(frameName, noFrames, _pollingTask);
        }


        private async Task Restore()
        {
            _driver.SendKeyToCanvas("restore");
            _driver.SendKeyToCanvas(Keys.Enter);
            await WaitAdditionalFrames(5);

            _driver.SendKeyToCanvas(Keys.Enter);
            await WaitAdditionalFrames(5);
            _driver.SendKeyToCanvas(Keys.Enter);
        }

        //private async Task Save()
        //{
        //    _driver.SendKeyToCanvas("save");
        //    _driver.SendKeyToCanvas(Keys.Enter);
        //    await Task.Delay(2000);

        //    _wrapper.EnqueueGameEvent(new SendString("\r"));
        //    await Task.Delay(5000);
        //    _wrapper.EnqueueGameEvent(new SendString("Test"));

        //    _wrapper.EnqueueGameEvent(new SendString("\r"));

        //    await Task.Delay(2000);
        //    _wrapper.EnqueueGameEvent(new SendString("\r"));
        //    await Task.Delay(2000);
        //}



        protected override void Quit()
        {

        }

        private void SetupPollingTask(string expectedFrameName, int noFrames)
        {
            string text = File.ReadAllText("CanvasCapture.js");
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)_driver;
            _pollingTask = Task.Run(
                  () =>
                  {
                      byte[] previousResult = null;
                      while (GetNoCapturedFrames() < noFrames)
                      {
                          Dictionary<string, object> result = (Dictionary<string, object>)javaScriptExecutor.ExecuteScript(text);

                          if (result != null)
                          {
                              byte[] data = ((IReadOnlyCollection<object>)result["data"]).Select(o => (byte)((long)o)).ToArray();
                              if (previousResult == null || !data.SequenceEqual(previousResult))
                              {
                                  CapturedAndQuit(data, 0, 0, (int)((long)result["width"]), (int)((long)result["height"]), noFrames, expectedFrameName);
                              }
                          }
                      }
                  }
          );
        }

        protected override Task WaitForExpectedFrameAndQuit(string expectedFrameName, int noFrames, Task completeWhenQuit, int delay = 30000, int quitDelay = 30000)
        {
            return base.WaitForExpectedFrameAndQuit(expectedFrameName, noFrames, completeWhenQuit, delay, quitDelay);
        }

    }
}
