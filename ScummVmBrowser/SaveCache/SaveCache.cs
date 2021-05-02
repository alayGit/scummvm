using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedCommon.Interfaces;
using ManagedCommon.Models;
using Newtonsoft.Json;
using SevenZCompression;
using ManagedYEncoder;
using ManagedCommon.Enums.Logging;
using System.Diagnostics;

namespace GameSaveCache
{
	public class SaveCache : ISaveCache
	{
		private IDictionary<string, GameSave> _cache;
		public const string NotInitedError = "The cache has not yet being set.";
		ISaveDataEncoderAndDecompresser _saveDataEncoderAndDecompresser;

		public SaveCache(ISaveDataEncoderAndDecompresser saveDataEncoderAndDecompresser)
		{
			_saveDataEncoderAndDecompresser = saveDataEncoderAndDecompresser;
			_cache = new Dictionary<string, GameSave>();
		}

		public string GetCompressedAndEncodedSaveData()
		{
			return _saveDataEncoderAndDecompresser.CompressAndEncode(_cache);
		}

		public IEnumerable<string> ListCache()
		{
			return _cache.Keys;
		}

		public void RemoveFromCache(string name)
		{
			_cache.Remove(name);
		}

		public void SaveToCache(string name, GameSave saveData)
		{
			_cache[name] = saveData;
		}

		public void SetCache(string encodedCompressedCache)
		{
			if (!String.IsNullOrEmpty(encodedCompressedCache))
			{
				_cache = _saveDataEncoderAndDecompresser.DecompressAndDecode(encodedCompressedCache);
			}
			else
			{
				_cache = new Dictionary<string, GameSave>();
			}
		}

		public byte[] GetFromCache(string name)
		{
			byte[] result = null;

			if (_cache.ContainsKey(name))
			{
				result = _cache[name].Data;
			}

			return result;
		}

		public int SaveCount
		{
			get
			{
				return _cache.Count();
			}
		}
	}
}
