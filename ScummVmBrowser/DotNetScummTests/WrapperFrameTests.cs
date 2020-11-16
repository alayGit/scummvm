using CLIScumm;
using CliScummEvents;
using ConfigStore;
using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Implementations;
using ManagedCommon.Interfaces;
using ManagedZLibCompression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetScummTests
{
    [TestClass]
    public class WrapperFrameTests : FrameCaptureTests
    {
        Wrapper _wrapper;
        Task gameTask;
        ConcurrentDictionary<string, byte[]> _saveData;

        const string ExpectedSaveFilePrefix = "kq3.";
        const string SaveDataResourceName = "SaveData";



        byte[] ExpectedAGISaveDataPrefix = new byte[] { 65, 71, 73, 58 }; //All AGI saves start with this

		public void Setup(String gameFolderLocation, CopyRectToScreen copyRectToScreen)
		{
			ManagedZLibCompression.ManagedZLibCompression managedZLibCompression = new ManagedZLibCompression.ManagedZLibCompression();

			_saveData = new ConcurrentDictionary<string, byte[]>();
			_wrapper = new Wrapper(new JsonConfigStore());

			_wrapper.OnCopyRectToScreen += (List<ScreenBuffer> l) => copyRectToScreen(
				l.Select(d => new ScreenBuffer()
				    {
						Buffer = managedZLibCompression.Decompress(d.Buffer), H = d.H, W = d.W, X = d.X, Y = d.Y
					}
		      ).ToList()
			);

            _wrapper.OnSaveData += (byte[] data, string fileName) => {
                _saveData[fileName] = data;
                return true;
             };
            gameTask = Task.Run(() => RunGame());
        }

        [TestMethod]
        public async Task CanStart()
        {
            Cropping = null;
            const string expectedFrameName = "CanStart";
            const int noFrames = 100;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName) );
            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

		[TestMethod]
		public async Task DoesDisplayBlackFirstFrame()
		{
			Cropping = null;
			const string expectedFrameName = "DoesDisplayBlackFirstFrame";
			const int noFrames = 1;
			//DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
			Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName));
			await CheckForExpectedFrame(expectedFrameName, noFrames);
		}

		[TestMethod]
        public async Task CanStartWholeFrame()
        {
            Cropping = null;
            const string expectedFrameName = "CanStart";
            const int noFrames = 100;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuitWholeFrame(noFrames, expectedFrameName));
            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendEnter()
        {
            const string expectedFrameName = "CanSendEnter";
            const int noFrames = 100;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendEsc()
        {
            Cropping = new Rectangle(0, 0, 98, 35);
            const string expectedFrameName = "CanSendEsc";
            const int noFrames = 175;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Escape));

            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Escape));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendF1_Help()
        {
            Cropping = null;
            const string expectedFrameName = "CanSendF1";
            const int noFrames = 100;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.F1));

            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendF2_Sound()
        {
            Cropping = AgiTitleOnly;
            const string expectedFrameName = "CanSendF2";
            const int noFrames = 100;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10, false);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.F2));
            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        //F3 Is Repeat Which Requires That ASCII be done first 

        [TestMethod]
        public async Task CanSendF4_Inventory()
        {
            Cropping = null;
            const string expectedFrameName = "CanSendF4";
            const int noFrames = 100;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.F4));

            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendString()
        {
            Cropping = new Rectangle(78, 86, 32, 14);
            const string expectedFrameName = "CanSendString";
            const int noFrames = 240;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendString("AbcdEFg"));
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await WaitForFrame(40);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendDownArrow()
        {
            Cropping = new Rectangle(55, 78, 30, 9);
            const string expectedFrameName = "CanSendDownArrow";
            const int noFrames = 125;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.ArrowDown));
            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendUp()
        {
            Cropping = null;
            Cropping = new Rectangle(0, 0, 320, 41);
            const string expectedFrameName = "CanSendUp";
            const int noFrames = 400;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.ArrowUp));
            await WaitForFrame(100);

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendLeft()
        {
            Cropping = null;
            //Cropping = new Rectangle(0, 8,320, 42);
            const string expectedFrameName = "CanSendLeft";
            const int noFrames = 300;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await WaitAdditionalFrames(10);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Escape));
            await WaitForFrame(25);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.ArrowLeft));

            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Escape));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendRight()
        {
            Cropping = new Rectangle(55, 0, 155, 80);
            const string expectedFrameName = "CanSendRight";
            const int noFrames = 300;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await WaitAdditionalFrames(10);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Escape));
            await Task.Delay(2000);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.ArrowRight));

            await Task.Delay(3000);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Escape));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendBackspace()
        {
            Cropping = new Rectangle(86, 83, 138, 18);
            const string expectedFrameName = "CanSendBackspace";
            const int noFrames = 140;

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));

            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await WaitAdditionalFrames(10);
            _wrapper.EnqueueGameEvent(new SendString("AbcdEFg"));
            await WaitAdditionalFrames(10);
            _wrapper.EnqueueGameEvent(new SendString("\b"));

            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendBackspaceViaControlKey()
        {
            Cropping = new Rectangle(86, 83, 138, 18);
            const string expectedFrameName = "CanSendBackspace";
            const int noFrames = 140;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await WaitAdditionalFrames(10);
            _wrapper.EnqueueGameEvent(new SendString("AbcdEFg"));
            await WaitAdditionalFrames(10);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Backspace));

            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendTab()
        {
            Cropping = null;
            const string expectedFrameName = "CanSendTab";
            const int noFrames = 150;

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await WaitAdditionalFrames(10);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Tab));

            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await Task.Delay(1000);
            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }


        [TestMethod]
        public async Task CanMoveMouse()
        {
            Cropping = new Rectangle(28, 0, 51, 35);
            const string expectedFrameName = "CanMoveMouse";
            const int noFrames = 100;

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendMouseMove(44,15));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

		[TestMethod]
		public async Task CanMoveMouseMultipleTimes()
		{
			Cropping = new Rectangle(93, 162, 15, 15);
			const string expectedFrameName = "CanMoveMouseMultipleTimes";
			const int noFrames = 100;

			Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName));
			await WaitForFrame(10);
			_wrapper.EnqueueGameEvent(new SendMouseMove(44, 15));
			_wrapper.EnqueueGameEvent(new SendMouseMove(12, 35));
			_wrapper.EnqueueGameEvent(new SendMouseMove(72, 55));
			_wrapper.EnqueueGameEvent(new SendMouseMove(100, 160));

			await CheckForExpectedFrame(expectedFrameName, noFrames);
		}

		[TestMethod]
        public async Task CanMoveMouseFarLeft()
        {
            Cropping = new Rectangle(0,0,30,55);
            const string expectedFrameName = "CanMoveMouseFarLeft";
            const int noFrames = 100;

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendMouseMove(0, 15));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanMoveMouseFarRight()
        {
            Cropping = new Rectangle(300,15,20,55);
            const string expectedFrameName = "CanMoveMouseFarRight";
            const int noFrames = 100;

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendMouseMove(320, 15));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanMoveMouseFarTop()
        {
            Cropping = new Rectangle(0, 0, 55, 55); 
            const string expectedFrameName = "CanMoveMouseFarTop";
            const int noFrames = 100;

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendMouseMove(15, 0));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanMoveMouseFarBottom()
        {
            Cropping = new Rectangle(0, 180, 55, 20);
            const string expectedFrameName = "CanMoveMouseFarBottom";
            const int noFrames = 100;

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendMouseMove(15, 200));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CursorCropsFarRight()
        {
            Cropping = new Rectangle(300, 15, 20, 55);
            const string expectedFrameName = "CursorCropsFarRight";
            const int noFrames = 100;

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendMouseMove(315, 15));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CursorCropsFarBottom()
        {
            Cropping = new Rectangle(0, 180, 55, 20);
            const string expectedFrameName = "CanMoveMouseFarBottom";
            const int noFrames = 100;

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendMouseMove(15, 195));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanClick()
        {
            Cropping = null;
            const string expectedFrameName = "CanSendEnter";
            const int noFrames = 200;

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendMouseClick(ManagedCommon.Enums.Actions.MouseClick.Left, new GetCurrentMousePosition(() => _wrapper.GetCurrentMousePosition())));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }


        [TestMethod]
        public async Task CanSave()
        {
            const int noFrames = 1000;
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, "_"));//Not checking against the capture frames

            await WaitForFrame(10);

            _wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.ArrowLeft));

            await WaitAdditionalFrames(10);

            await Save();

            await Task.Delay(2000);

            string expectedSaveFileName = $"{ExpectedSaveFilePrefix}001";

            Assert.IsTrue(_saveData.ContainsKey(expectedSaveFileName));

            byte[] actualSaveDataPrefix = new byte[ExpectedAGISaveDataPrefix.Length];

            Array.Copy(_saveData[expectedSaveFileName], actualSaveDataPrefix, ExpectedAGISaveDataPrefix.Length);

            Assert.IsTrue(ExpectedAGISaveDataPrefix.SequenceEqual(actualSaveDataPrefix));

            await QuitAsync();
        }

        [TestMethod]
        public async Task CanRestore()
        {
            Cropping = new Rectangle(0, 10, 100, 30);
            const int noFrames = 1000;
            const string expectedFrameName = "CanRestore";

            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CapturedAndQuit(screenBuffers[0].Buffer, screenBuffers[0].X, screenBuffers[0].Y, screenBuffers[0].W, screenBuffers[0].H, noFrames, expectedFrameName));//Not checking against the capture frames

            await WaitForFrame(10);

            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await WaitAdditionalFrames(10);

            await Restore();
            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        private async Task Save()
        {
            _wrapper.EnqueueGameEvent(new SendString("save"));
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await Task.Delay(2000);

            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await Task.Delay(5000);
            _wrapper.EnqueueGameEvent(new SendString("Test"));

            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await Task.Delay(2000);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await Task.Delay(2000);
        }

        private async Task Restore()
        {
            _wrapper.EnqueueGameEvent(new SendString("restore"));
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await Task.Delay(2000);

            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await Task.Delay(2000);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
        }

        private async Task CheckForExpectedFrame(string expectedFrameName, int noFrames, int delay = 20000)
        {
            await WaitForExpectedFrameAndQuit(expectedFrameName, noFrames, gameTask, delay);
        }

        private void RunGame()
        {
            _saveData = GetSaveDataFromResourceFile();
            _wrapper.RunGame(AvailableGames.kq3, null, new Dictionary<string, byte[]>(_saveData), (byte[] aud) => { });
        }

        protected override void Quit()
        {
            _wrapper.Quit();
        }

        private async Task QuitAsync()
        {
            Quit();
            await gameTask;
        }

        protected void CapturedAndQuitWholeFrame(int noFrames, string expectedFrameName)
        {
            int width = 0, height = 0;

            base.CaptureAndQuitWholeFrame(new ManagedZLibCompression.ManagedZLibCompression().Decompress(_wrapper.GetWholeScreen(ref width, ref height)), 0, 0, width, height, noFrames, expectedFrameName);
        }
    }
}
