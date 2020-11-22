using ManagedCommon.Interfaces;
using Microsoft.AspNet.SignalR.Client.Transports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetScummTests
{
	public abstract class FrameCaptureTests
	{
		ConcurrentQueue<Bitmap> _capturedFrames;
		Dictionary<uint, Dictionary<int, byte[]>> _palettes;
		bool hasSentQuit;

		protected const string gameDirectory = "C:\\scumm\\GamesKQ3";
		bool saveEnabled = true;
		public const int DisplayDefaultWidth = 320;
		public const int DisplayDefaultHeight = 200;
		public const string GameDirectory = "C:\\scumm\\GamesKQ3";
		public const int NoBytesPerPixel = 4;
		public const int NoColors = 256;

		const string SaveDataResourceName = "SaveData";
		protected static ResourceManager ResourceManager = DotNetScummTests.Properties.Resources.ResourceManager;
		private object _clearLock;


		Bitmap _b;

		enum ARBG
		{
			R = 2,
			G = 1,
			B = 0,
			A = 3
		}

		protected FrameCaptureTests()
		{
			_b = new Bitmap(DisplayDefaultWidth, DisplayDefaultHeight);
			_clearLock = new object();
			_palettes = new Dictionary<uint, Dictionary<int, byte[]>>();
		}

		protected Rectangle? Cropping { get; set; }

		public static Rectangle? AgiNoTitleNoMouse //Mouse is up the top and gets cropped
		{
			get
			{
				return new Rectangle(0, 20, 320, 180);
			}
		}

		public static Rectangle? AgiTitleOnly
		{
			get
			{
				return new Rectangle(0, 0, 320, 10);
			}
		}

		[TestInitialize]
		public void StartTests()
		{
			_capturedFrames = new ConcurrentQueue<Bitmap>();
			hasSentQuit = false;
			Cropping = AgiNoTitleNoMouse;
		}


		private Bitmap Crop(Bitmap toCrop)
		{
			try
			{
				return toCrop.Clone(Cropping.Value, toCrop.PixelFormat);
			}
			catch (Exception e)
			{
				e.GetHashCode();
				throw;
			}
		}

		public bool AssertThatExpectedFrameIsSeen(string expectedResourceName)
		{
			lock (_clearLock)
			{
				bool expectedFrameFound = false;
				IEnumerator<Bitmap> enumerator = _capturedFrames.ToList().GetEnumerator();

				Bitmap expectedBitmap = (Bitmap)ResourceManager.GetObject($"{expectedResourceName}");
				bool alreadyCropped = false;

				//expectedBitmap = new Bitmap(expectedBitmap, new Size(DisplayDefaultWidth, DisplayDefaultHeight)); //Sometimes the Bitmap library can shrink the image causing issues

				while (enumerator.MoveNext() && !expectedFrameFound)
				{
					Bitmap actualBitmap = enumerator.Current;

					if (expectedBitmap == null)
					{
						throw new Exception($"Check that {expectedResourceName} exists");
					}

					if (Cropping != null && !alreadyCropped)
					{
						expectedBitmap = Crop(expectedBitmap);
						alreadyCropped = true;
					}

					bool frameCouldBeExpected = true; //If the sizes don't match we are not even going to bother checking. Some frames such as the inventory screen in KQ3 are different sizes to the rest of the game
					for (int x = 0; x < actualBitmap.Size.Width && frameCouldBeExpected; x++)
					{
						for (int y = 0; y < actualBitmap.Size.Height && frameCouldBeExpected; y++)
						{
							frameCouldBeExpected = expectedBitmap.GetPixel(x, y) == actualBitmap.GetPixel(x, y);
						}
					}
					expectedFrameFound = frameCouldBeExpected;
				}
				return expectedFrameFound;
			}
		}

		protected async Task WaitForFrame(int frame, bool soundOff = true)
		{
			while (_capturedFrames.Count < frame)
			{
				await Task.Delay(10);
				//wrapper.EnqueueGameEvent(new SendControlCharacters(ControlKeys.F2));
			}
		}

		protected async Task WaitAdditionalFrames(int additionalFrames, bool soundOff = true)
		{
			int initialCurrentFrame = _capturedFrames.Count;
			while (_capturedFrames.Count < initialCurrentFrame + additionalFrames)
			{
				await Task.Delay(10);
			}
		}

		protected virtual void CaptureAndQuitWholeFrame(byte[] picBuff, UInt32 paletteHash, int x, int y, int w, int h, int noFrames, string expectedFrameName)
		{
			_b = new Bitmap(DisplayDefaultWidth, DisplayDefaultHeight);
			CaptureAndQuit(picBuff, paletteHash, x, y, w, h, noFrames, expectedFrameName);
		}

		static int counter = 0;
		protected unsafe virtual void CaptureAndQuit(byte[] picBuff, UInt32 paletteHash, int x, int y, int w, int h, int noFrames, string expectedFrameName)
		{
			BitmapData bitmapData = null;
			try
			{
				if (w > 0 && h > 0)
				{
					int byteNo = 0;
					bitmapData = _b.LockBits(new Rectangle(x, y, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
					byte* scan0 = (byte*)bitmapData.Scan0.ToPointer();
					for (int heightCounter = 0; heightCounter < h; heightCounter++)
					{
						for (int widthCounter = 0; widthCounter < w; widthCounter++, byteNo++)
						{
							byte[] colourComponents = _palettes[paletteHash][picBuff[byteNo]];
							byte* data = scan0 + heightCounter * bitmapData.Stride + widthCounter * 4;
							*(data + (int)ARBG.A) = colourComponents[0];
							*(data + (int)ARBG.R) = colourComponents[1];
							*(data + (int)ARBG.G) = colourComponents[2];
							*(data + (int)ARBG.B) = colourComponents[3];
						}
					}
					CaptureAndQuit(_b, noFrames, expectedFrameName);
				}
			}
			catch (Exception e)
			{
				int ax = 4;
			}
			finally
			{
				if (bitmapData != null)
				{
					_b.UnlockBits(bitmapData);
				}
			}
		}

		private void CaptureAndQuit(Bitmap bitmap, int noFrames, string expectedFrameName)
		{
			if (_capturedFrames.Count < noFrames)
			{
				if (saveEnabled)
				{
					if (Cropping != null)
					{
						bitmap = Crop(bitmap);
					}
					bitmap.Save($"C:\\temp\\First100\\{expectedFrameName}_{_capturedFrames.Count}.bmp");
					counter++;
				}
				CopyRectToQueue(bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat));
			}
			else if (_capturedFrames.Count == noFrames && !hasSentQuit)
			{
				Quit();
				hasSentQuit = true;
			}
		}

		protected void CaptureAndQuit(List<ScreenBuffer> screenBuffers, int noFrames, string expectedFrameName)
		{
			ManagedZLibCompression.ManagedZLibCompression compression = new ManagedZLibCompression.ManagedZLibCompression();
			screenBuffers.Where(b => b.CompressedPaletteBuffer != null).ToList().ForEach(b => UpdatePalettes(b.PaletteHash, compression.Decompress(b.CompressedPaletteBuffer)));

			foreach (ScreenBuffer screenBuffer in screenBuffers)
			{
				CaptureAndQuit(compression.Decompress(screenBuffer.CompressedBuffer), screenBuffer.PaletteHash, screenBuffer.X, screenBuffer.Y, screenBuffer.W, screenBuffer.H, noFrames, expectedFrameName);
			}
		}

		private Dictionary<int, byte[]> InitializePaletteHashDictionary()
		{
			Dictionary<int, byte[]> result = new Dictionary<int, byte[]>();
			for (int i = 0; i < NoColors; i++)
			{
				result[i] = new byte[NoBytesPerPixel];
			}

			return result;
		}

		private void UpdatePalettes(uint paletteHash, byte[] decompressedPalette)
		{
			string paletteString = Encoding.ASCII.GetString(decompressedPalette);
			_palettes[paletteHash] = InitializePaletteHashDictionary();

			if (paletteString.Length % (3 * NoBytesPerPixel) != 0)
			{
				throw new Exception("String is the wrong length");
			}

			int paletteNo = 0;
			string digit = "";
			int colourComponent = 0;
			foreach (char c in paletteString)
			{
				try
				{
					if (digit.Length < 3)
					{
						digit += c.ToString();
					}
					else
					{
						int iDigit = int.Parse(digit);

						if (iDigit < Byte.MinValue || iDigit > Byte.MaxValue)
						{
							throw new Exception("Fail out of range digit");
						}

						_palettes[paletteHash][paletteNo][colourComponent] = Byte.Parse(digit);

						colourComponent = (colourComponent + 1) % NoBytesPerPixel;

						if (colourComponent == 0)
						{
							paletteNo = (paletteNo + 1) % NoColors;
						}

						digit = c.ToString();
					}
				}
				catch (Exception e)
				{
					int x = 4;
				}
			}
		}


		public void CopyRectToQueue(Bitmap bitmap)
		{
			lock (_clearLock)
			{
				_capturedFrames.Enqueue(bitmap);
			}
		}

		protected virtual async Task WaitForExpectedFrameAndQuit(string expectedFrameName, int noFrames, Task completeWhenQuit, int delay = 20000000, int quitDelay = 20000000)
		{
			Task checkForFrame = Task.Run(async () =>
			{
				while (!AssertThatExpectedFrameIsSeen(expectedFrameName))
				{
					await Task.Delay(10);
				}
			});

			await Task.WhenAny(checkForFrame, Task.Delay(delay));

			if (!checkForFrame.IsCompleted)
			{
				throw new Exception($"Did not display first {noFrames} frames in time");
			}

			if (checkForFrame.IsFaulted)
			{
				throw checkForFrame.Exception;
			}

			await Task.WhenAny(completeWhenQuit, Task.Delay(quitDelay));

			if (!completeWhenQuit.IsCompleted)
			{
				throw new Exception("Did not quit in time");
			}

			if (completeWhenQuit.IsFaulted)
			{
				throw completeWhenQuit.Exception;
			}
		}

		protected int GetNoCapturedFrames()
		{
			return _capturedFrames.Count;
		}

		protected void ClearFrames()
		{
			lock (_capturedFrames)
			{
				_capturedFrames = new ConcurrentQueue<Bitmap>();
			}
		}

		protected ConcurrentDictionary<string, byte[]> GetSaveDataFromResourceFile()
		{
			string jsonData = GetJsonData();

			return JsonConvert.DeserializeObject<ConcurrentDictionary<string, byte[]>>(jsonData);
		}

		protected string GetJsonData()
		{
			return (string)ResourceManager.GetObject($"{SaveDataResourceName}");
		}

		protected abstract void Quit();
	}
}
