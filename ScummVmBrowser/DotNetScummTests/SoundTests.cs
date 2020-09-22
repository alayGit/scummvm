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
	public class SoundTests
	{
		private Wrapper _wrapper;
		private Task _gameTask;
		private bool? _allBeginWithPreamble;
		
		private readonly byte[] FlacPreamble = { 102, 76, 97, 67 };


		[TestInitialize]
		public void Setup()
		{
			ManagedZLibCompression.ManagedZLibCompression managedZLibCompression = new ManagedZLibCompression.ManagedZLibCompression();

			_wrapper = new Wrapper(new JsonConfigStore());

			_wrapper.OnCopyRectToScreen += (List<ScreenBuffer> l) => { };

			_wrapper.OnSaveData += (byte[] data, string fileName) => true;

			_allBeginWithPreamble = null;
		}

		[TestCleanup]
		public void Cleanup()
		{
			_wrapper.Dispose();
		}

		[TestMethod]
		public async Task DoesSoundContainExpectedValue()
		{
			_gameTask = Task.Run(() => RunGame());
			_wrapper.StartSound();

			await Task.Delay(1000);

			_wrapper.Quit();

			Assert.IsTrue(_allBeginWithPreamble.HasValue && _allBeginWithPreamble.Value);

			await _gameTask;
		}

		private void RunGame()
		{
			_wrapper.RunGame(AvailableGames.kq3, null, new Dictionary<string, byte[]>(), (byte[] aud) => {
				_allBeginWithPreamble = new ManagedZLibCompression.ManagedZLibCompression().Decompress(aud).Take(FlacPreamble.Length).SequenceEqual(FlacPreamble) && (!_allBeginWithPreamble.HasValue || _allBeginWithPreamble.Value);
		  });
		}

	}
}
