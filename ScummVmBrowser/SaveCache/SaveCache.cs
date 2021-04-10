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

namespace SaveCache
{
	public class SaveCache : ISaveCache
	{
		private Dictionary<string, GameSave> _cache;
		public const string NotInitedError = "The cache has not yet being set.";
		SevenZCompressor _sevenZCompressor;
		ManagedYEncoder.ManagedYEncoder yEncoder;

		public SaveCache(ILogger logger)
		{
			_sevenZCompressor = new SevenZCompressor();
			yEncoder = new ManagedYEncoder.ManagedYEncoder(logger, LoggingCategory.CliScummSelfHost); //TODO: Fix category
			_cache = new Dictionary<string, GameSave>();
		}

		public string GetCompressedAndEncodedSaveData()
		{
			string serializedCache = JsonConvert.SerializeObject(_cache);
			byte[] compressed = _sevenZCompressor.Compress(Encoding.UTF8.GetBytes(serializedCache), 10);
			return  yEncoder.ByteEncode(compressed);
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

		public void SetCache(string yEncodedCompressedCache)
		{
			byte[] compressedCache = yEncoder.ByteDecode(yEncodedCompressedCache);
			string serializedCache = yEncoder.TextEncoding.GetString(_sevenZCompressor.Decompress(compressedCache));

			_cache = JsonConvert.DeserializeObject<Dictionary<string, GameSave>>(serializedCache);
		}

		public byte[] GetFromCache(string name)
		{
			return _cache[name].Data;
		}
	}
}
