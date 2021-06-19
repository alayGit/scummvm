using CLIScumm;
using CliScummEvents;
using ConfigStore;
using GameSaveCache;
using ManagedCommon.Constants;
using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using ManagedCommon.Enums.Logging;
using ManagedCommon.Enums.Other;
using ManagedCommon.Implementations;
using ManagedCommon.Interfaces;
using ManagedCommon.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Saving;
using ScummTimer;
using SevenZCompression;
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
        string _saveData;
		ISaveDataEncoderAndDecompresser _saveDataEncoderAndDecompresser;
		ISaveDataEncoder _byteEncoder;
		ISaveDataCompression _compressor;
		IConfigurationStore<System.Enum> _configStore;
		SaveCache _saveCache;

        const string ExpectedSaveFilePrefix = "kq3.";
        const string SaveDataResourceName = "SaveData";
		const string KingsQuest4CopyProtectionWord = "Game";
		const int Kq4HotCursorOffset = 8;
		const string ThumbnailResourceName = "Thumbnail";



        byte[] ExpectedAGISaveDataPrefix = new byte[] { 65, 71, 73, 58 }; //All AGI saves start with this

		[TestInitialize]
		public void Init()
		{
			System.IO.File.Delete("scummvm.ini");
		}

		public void Setup(String gameFolderLocation, int noFrames, string expectedFrameName, AvailableGames game = AvailableGames.kq3, string saveDataResourceName = DefaultSavesDataResourceName, uint saveSlotToLoad = Constants.DoNotLoadSaveSlot)
		{
			Setup(gameFolderLocation, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName), game, saveDataResourceName, saveSlotToLoad);
		}

		public void Setup(String gameFolderLocation, SendScreenBuffers copyRectToScreen, AvailableGames game = AvailableGames.kq3, string saveDataResourceName = DefaultSavesDataResourceName, uint saveSlotToLoad = Constants.DoNotLoadSaveSlot)
		{
			ILogger logger = new Mock<ILogger>().Object;
			_byteEncoder = new Base64ByteEncoder.Base64ByteEncoder();
			_compressor = new SevenZCompressor();
			_configStore = new JsonConfigStore();
			_saveDataEncoderAndDecompresser = new SaveDataEncoderAndCompressor(_byteEncoder, _compressor, _configStore);
			_saveCache = new SaveCache(_saveDataEncoderAndDecompresser);

			_wrapper = new Wrapper(new JsonConfigStore(), _saveCache , new Base64ByteEncoder.Base64ByteEncoder(), new ManagedScummTimer(new Mock<ILogger>().Object));

			_wrapper.SendScreenBuffers += (List<ScreenBuffer> l) => copyRectToScreen(l);

            _wrapper.OnSaveData += (string data) => {
                _saveData = data;
                return true;
             };
            gameTask = Task.Run(() => RunGame(game,saveDataResourceName, saveSlotToLoad));
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

		//Also tests steel
		[TestMethod]
		public async Task CanRunGamesRequiringReentrantMutexes()
		{
			const int MouseX = 172;
			const int MouseY = 173;

			Cropping = new Rectangle(0, 180, 10, 20);
			const string expectedFrameName = "CanRunGamesRequiringReentrantMutexes";
			const int noFrames = 2549;
			Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName), AvailableGames.steel);
			await WaitForFrame(50);

			_wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Escape));
			await WaitForFrame(300);
			_wrapper.EnqueueGameEvent(new SendMouseMove(MouseX,MouseY));
			await WaitForFrame(50);
			EnqueueClick(MouseX, MouseY);

			await CheckForExpectedFrame(expectedFrameName, noFrames);
		}


		[TestMethod]
		public async Task CanMoveMousePaletteDisabled()
		{
			Cropping = null;
			const string expectedFrameName = "CanMoveMousePaletteDisabled";
			const int noFrames = 25;

			Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName), AvailableGames.kq4);
			await WaitForFrame(10);
			//_wrapper.EnqueueGameEvent(new SendMouseMove(DisplayDefaultWidth + Kq4HotCursorOffset - 1, DisplayDefaultHeight + Kq4HotCursorOffset - 1));
			_wrapper.EnqueueGameEvent(new SendMouseMove(DisplayDefaultWidth + Kq4HotCursorOffset - 1, DisplayDefaultHeight + Kq4HotCursorOffset - 1));
			await WaitAdditionalFrames(10);
			_wrapper.EnqueueGameEvent(new SendMouseMove(Kq4HotCursorOffset, Kq4HotCursorOffset));
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
		public async Task CanRunGameRequiringLockScreen()
		{
			Cropping = new Rectangle(100, 100, 20, 20);
			const string expectedFrameName = "CanRunGameRequiringLockScreen";
			const int noFrames = 530;
			Setup(gameDirectory, noFrames, expectedFrameName, AvailableGames.kq4, KingsQuest4OnMountain, 1);
			await CheckForExpectedFrame(expectedFrameName, noFrames);
		}

		[TestMethod]
		public async Task CanStartKq5()
		{
			Cropping = new Rectangle(100, 100, 20, 20);
			const string expectedFrameName = "CanStartKq5";
			const int noFrames = 50;
			Setup(gameDirectory, noFrames, expectedFrameName, AvailableGames.kq5, Kq5CanStart, 1);
			await CheckForExpectedFrame(expectedFrameName, noFrames);
		}

		[TestMethod]
		public async Task CanStartKq7()
		{
			//Cropping = new Rectangle(100, 100, 20, 20);
			const string expectedFrameName = "CanStartKq5";
			const int noFrames = 50000;
			Setup(gameDirectory, noFrames, expectedFrameName, AvailableGames.kq7, Kq5CanStart, 1);
			await CheckForExpectedFrame(expectedFrameName, noFrames);
		}

		[TestMethod]
		public async Task CanRunGamesWithMusicTimer()
		{
			const string expectedFrameName = "CanRunGamesWithMusicTimer";
			const int noFrames = 1500;
			Cropping = new Rectangle(238, 20, 50, 20);
			//Cropping = new Rectangle(238, 40, 50, 20);
			Setup(gameDirectory, noFrames, expectedFrameName, AvailableGames.smi, Kq5CanStart, Constants.DoNotLoadSaveSlot);
			await CheckForExpectedFrame(expectedFrameName, noFrames);
		}


		[TestMethod]
		public async Task CanClickNonZeroHotSpot()
		{
			Cropping = null;
			const string expectedFrameName = "CanClickNonZeroHotSpot";
			const int noFrames = 800;

			Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuit(screenBuffers, noFrames, expectedFrameName), AvailableGames.kq4);
			await WaitForFrame(10);
			_wrapper.EnqueueGameEvent(new SendString(KingsQuest4CopyProtectionWord));
			_wrapper.EnqueueGameEvent(new SendString("\r"));
			await WaitForFrame(785);
			_wrapper.EnqueueGameEvent(new SendMouseMove(226, 120));
			//_wrapper.EnqueueGameEvent(new SendMouseClick(MouseClick.Left, () => new Point(226, 120)));

			await CheckForExpectedFrame(expectedFrameName, noFrames);
		}

		[TestMethod]
        public async Task CanStartWholeFrame()
        {
            Cropping = null;
            const string expectedFrameName = "CanStart";
            const int noFrames = 100;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, (List<ScreenBuffer> screenBuffers) => CaptureAndQuitWholeFrame(screenBuffers, noFrames, expectedFrameName));
			await WaitForFrame(10);
			_wrapper.ScheduleRedrawWholeScreen();
            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendEnter()
        {
			Cropping = new Rectangle(5, 0, 10, 10);
            const string expectedFrameName = "CanSendEnter";
            const int noFrames = 150;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendEsc()
        {
			Cropping = new Rectangle(5, 0, 10, 30);
			const string expectedFrameName = "CanSendEsc";
            const int noFrames = 310;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(180);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Escape));

            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Escape));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendF1_Help()
        {
            const string expectedFrameName = "CanSendF1";
            const int noFrames = 700;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(180);
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
            const int noFrames = 500;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(180);
			_wrapper.EnqueueGameEvent(new SendMouseMove(100,100)); //Mouse is in the way
			_wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.F2));
            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        //F3 Is Repeat Which Requires That ASCII be done first 

        [TestMethod]
        public async Task CanSendF4_Inventory()
        {
			Cropping = new Rectangle(100, 100, 10, 10);
            const string expectedFrameName = "CanSendF4";
            const int noFrames = 350;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(250);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
			await WaitForFrame(300);
			_wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.F4));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendString()
        {
			Cropping = new Rectangle(86, 83, 100, 20);
			const string expectedFrameName = "CanSendString";
            const int noFrames = 120;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(10);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
			await WaitForFrame(70);
			_wrapper.EnqueueGameEvent(new SendString("AbcdEFg"));
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await WaitForFrame(115);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await Task.Delay(1000);
            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendDownArrow()
        {
			Cropping = new Rectangle(52, 68, 140, 20);
			const string expectedFrameName = "CanSendDownArrow";
            const int noFrames = 122;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(15);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
			await WaitAdditionalFrames(10);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.ArrowDown));
            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendUp()
        {
			Cropping = new Rectangle(41, 82, 30, 30);
			const string expectedFrameName = "CanSendUp";
            const int noFrames = 263;
            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(15);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await WaitForFrame(72);
			_wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.ArrowUp));
	
			await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendLeft()
        {
			Cropping = new Rectangle(0,0, 30, 10);
            const string expectedFrameName = "CanSendLeft";
			const int noFrames = 140;
            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(30);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await WaitAdditionalFrames(10);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.Escape));
            await WaitAdditionalFrames(10);
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.ArrowLeft));
			await Task.Delay(1000);

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }

        [TestMethod]
        public async Task CanSendRight()
        {
            Cropping = new Rectangle(55, 0, 155, 30);
            const string expectedFrameName = "CanSendRight";
            const int noFrames = 210;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, noFrames, expectedFrameName);
			_wrapper.EnqueueGameEvent(new SendMouseMove(100,100)); //Move mouse out of the way
			await WaitForFrame(20);
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
            const int noFrames = 300;

            Setup(gameDirectory, noFrames, expectedFrameName);

            await WaitForFrame(180);
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
            const int noFrames = 300;
            //DotNetScummTests.Properties.Resources.CanDoFirst100Frames__97_
            Setup(gameDirectory, noFrames, expectedFrameName);
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
            const string expectedFrameName = "CanSendTab";
            const int noFrames = 500;

            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(180);
            _wrapper.EnqueueGameEvent(new SendString("\r"));
            await WaitAdditionalFrames(50);
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
            const string expectedFrameName = "CanSendEnter";
            const int noFrames = 250;

            Setup(gameDirectory, noFrames, expectedFrameName);
            await WaitForFrame(180);
            //_wrapper.EnqueueGameEvent(new SendMouseClick(ManagedCommon.Enums.Actions.MouseClick.Left, new GetCurrentMousePosition(() => _wrapper.GetCurrentMousePosition())));

            await CheckForExpectedFrame(expectedFrameName, noFrames);
        }


        [TestMethod]
        public async Task CanSave()
        {
			Cropping = new Rectangle(0, 0, 160, 80);
            const int noFrames = 1000;
			Setup(gameDirectory, noFrames, "_");//Not checking against the capture frames

			await WaitForFrame(10);

            _wrapper.EnqueueGameEvent(new SendString("\r"));
            _wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.ArrowLeft));

            await WaitAdditionalFrames(10);

            await Save();

            await Task.Delay(2000);

            string expectedSaveFileName = $"{ExpectedSaveFilePrefix}001";

			IDictionary<string, GameSave> gameSaves = _saveDataEncoderAndDecompresser.DecompressAndDecode(_saveData);
			Bitmap thumbnailBitmap = new Bitmap(DisplayDefaultWidthThumbnail, DisplayDefaultHeightThumbnail);
			
			SetBitmapData(gameSaves[expectedSaveFileName].Thumbnail, thumbnailBitmap, NoIgnoreColor, GetPalette(gameSaves[expectedSaveFileName].PaletteString), 0, 0, DisplayDefaultWidthThumbnail, DisplayDefaultHeightThumbnail);
			thumbnailBitmap = Crop(thumbnailBitmap);
			thumbnailBitmap.Save($"C:\\temp\\First100\\Thumbnail.bmp");

			Assert.IsTrue(ArePicturesEqual(ThumbnailResourceName, thumbnailBitmap));

            Assert.IsTrue(gameSaves.ContainsKey(expectedSaveFileName));

            byte[] actualSaveDataPrefix = new byte[ExpectedAGISaveDataPrefix.Length];

            Array.Copy(gameSaves[expectedSaveFileName].Data, actualSaveDataPrefix, ExpectedAGISaveDataPrefix.Length);

            Assert.IsTrue(ExpectedAGISaveDataPrefix.SequenceEqual(actualSaveDataPrefix));

            await QuitAsync();
        }

        [TestMethod]
        public async Task CanRestore()
        {
			Cropping = new Rectangle(0, 20, 100, 30);
            const int noFrames = 1000;
            const string expectedFrameName = "CanRestore";

            Setup(gameDirectory, noFrames, expectedFrameName);

            await WaitForFrame(20);

            _wrapper.EnqueueGameEvent(new SendString("\r"));

            await WaitAdditionalFrames(160);

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

        private async Task CheckForExpectedFrame(string expectedFrameName, int noFrames, int delay = 30000000)
        {
            await WaitForExpectedFrameAndQuit(expectedFrameName, noFrames, gameTask, delay);
        }

        private void RunGame(AvailableGames game = AvailableGames.kq3, string saveDataResourceName = DefaultSavesDataResourceName, uint saveSlotToLoad = Constants.DoNotLoadSaveSlot)
        {
            _saveData = GetSaveDataFromResourceFile(saveDataResourceName);
            _wrapper.RunGame(game, null, _saveData, (byte[] aud) => { }, saveSlotToLoad);
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

		private void EnqueueClick(int x, int y)
		{
			_wrapper.EnqueueGameEvent(new SendMouseClick(MouseClick.Left, () => new Point(x, y), MouseUpDown.MouseDown));
			_wrapper.EnqueueGameEvent(new SendMouseClick(MouseClick.Left, () => new Point(x, y), MouseUpDown.MouseUp));
		}
    }
}
