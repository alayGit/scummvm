using GameSaveCache;
using ManagedCommon.Enums.Logging;
using ManagedCommon.Interfaces;
using ManagedCommon.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SevenZCompression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saving;
using ConfigStore;

namespace SaveCacheTests
{
    [TestClass]
    public class SaveCacheTests
    {
		SaveCache _saveCache;
		ISaveDataEncoderAndDecompresser _saveDataEncoderAndDecompresser;
		Base64ByteEncoder.Base64ByteEncoder _base64Encoder;
		SevenZCompressor _sevenZCompressor;
		ILogger _logger;
		IConfigurationStore<System.Enum> _configStore;

		GameSave GameSave1 = new GameSave() { Data = new byte[] { 12, 33, 33, 12 }, Thumbnail = new byte[] { 11, 33, 12 } };
		GameSave GameSave2 = new GameSave() { Data = new byte[] { 11, 23, 13, 133 }, Thumbnail = new byte[] { 1, 13, 2 } };
		GameSave GameSave3 = new GameSave() { Data = new byte[] { 1, 3, 13, 133 }, Thumbnail = new byte[] { 1, 3, 2 } };

		public SaveCacheTests()
		{
			_configStore = new Mock<IConfigurationStore<System.Enum>>().Object;
			_logger = new Mock<ILogger>().Object;
			_sevenZCompressor = new SevenZCompressor();
			_base64Encoder = new Base64ByteEncoder.Base64ByteEncoder();
			_saveDataEncoderAndDecompresser = new SaveDataEncoderAndCompressor(_base64Encoder, _sevenZCompressor, _configStore);
			_saveCache = new SaveCache(_saveDataEncoderAndDecompresser);
		}

		[TestMethod]
		public void NotSettingCacheMeansAnEmptyCache()
		{
			CheckSaveCacheEqual(new Dictionary<string, GameSave>(), _saveCache.GetCompressedAndEncodedSaveData());
		}

		[TestMethod]
        public void CanAddToCache()
        {
			_saveCache.SetCache(string.Empty);
			_saveCache.SaveToCache("1", GameSave1);
			_saveCache.SaveToCache("2", GameSave2);
			_saveCache.SaveToCache("3", GameSave3);

			Dictionary<string, GameSave> expected = new Dictionary<string, GameSave>();
			expected.Add("1", GameSave1);
			expected.Add("2", GameSave2);
			expected.Add("3", GameSave3);

			CheckSaveCacheEqual(expected, _saveCache.GetCompressedAndEncodedSaveData());
        }

		[TestMethod]
		public void CanRemoveFromCache()
		{
			_saveCache.SetCache(string.Empty);
			_saveCache.SaveToCache("1", GameSave1);
			_saveCache.SaveToCache("2", GameSave2);
			_saveCache.SaveToCache("3", GameSave3);
			_saveCache.RemoveFromCache("2");

			Dictionary<string, GameSave> expected = new Dictionary<string, GameSave>();
			expected.Add("1", GameSave1);
			expected.Add("3", GameSave3);

			CheckSaveCacheEqual(expected, _saveCache.GetCompressedAndEncodedSaveData());
		}

		[TestMethod]
		public void CanListCache()
		{
			_saveCache.SetCache(string.Empty);
			_saveCache.SaveToCache("1", GameSave1);
			_saveCache.SaveToCache("2", GameSave2);
			_saveCache.SaveToCache("3", GameSave3);

			int expectedKey = 1;
			foreach(string s in _saveCache.ListCache())
			{
				Assert.AreEqual(expectedKey.ToString(), s);
				expectedKey++;
			}
		}

		[TestMethod]
		public void SettingCacheToEmptyStringCreatesEmptyCache()
		{
			_saveCache.SetCache(string.Empty);
		
			CheckSaveCacheEqual(new Dictionary<string, GameSave>(), _saveCache.GetCompressedAndEncodedSaveData());
		}

		private void CheckSaveCacheEqual(Dictionary<string, GameSave> expected, string actual)
		{
			byte[] decoded = _base64Encoder.ByteDecode(actual);
			byte[] decompressed = _sevenZCompressor.Decompress(decoded);
			string json = Encoding.ASCII.GetString(decompressed);
			Dictionary<string, GameSave> dictActual = JsonConvert.DeserializeObject<Dictionary<string, GameSave>>(json);
			foreach (KeyValuePair<string, GameSave> kvp in expected)
			{
				Assert.IsTrue(kvp.Value.Data.SequenceEqual(dictActual[kvp.Key].Data));
				Assert.IsTrue(kvp.Value.Thumbnail.SequenceEqual(dictActual[kvp.Key].Thumbnail));
			}

			Assert.AreEqual(expected.Count(), dictActual.Count());
		}

		[TestMethod]
		public void CanSetCacheToNonEmptyValue()
		{
			Dictionary<string, GameSave> expected = new Dictionary<string, GameSave>();
			expected.Add("1", GameSave1);
			expected.Add("2", GameSave2);
			expected.Add("3", GameSave3);

			string json = JsonConvert.SerializeObject(expected);
			byte[] compressed = _sevenZCompressor.Compress(Encoding.ASCII.GetBytes(json), 10);
			string encoded = _base64Encoder.ByteEncode(compressed);

			_saveCache.SetCache(encoded);

			CheckSaveCacheEqual(expected, _saveCache.GetCompressedAndEncodedSaveData());
		}
	}
}
