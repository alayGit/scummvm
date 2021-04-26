using ManagedCommon.Delegates;
using ManagedCommon.Enums.Settings;
using ManagedCommon.Interfaces;
using ManagedCommon.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saving
{
	public class SaveDataEncoderAndCompressor : ISaveDataEncoderAndDecompresser
	{
		private IByteEncoder _byteEncoder;
		private ISaveDataCompression _saveDataCompression;
		private IConfigurationStore<System.Enum> _configStore;

		public SaveDataEncoderAndCompressor(IByteEncoder byteEncoder, ISaveDataCompression saveDataCompression, IConfigurationStore<System.Enum> configStore)
		{
			_byteEncoder = byteEncoder;
			_saveDataCompression = saveDataCompression;
			_configStore = configStore;
		}

		public string CompressAndEncode(IDictionary<string, GameSave> saveData)
		{
			string json = JsonConvert.SerializeObject(saveData);
			byte[] compressed = _saveDataCompression.Compress(Encoding.UTF8.GetBytes(json), _configStore.GetValue<uint>(SaveCompressionSettings.CompressionLevel));
			return _byteEncoder.ByteEncode(compressed);
		}

		public IDictionary<string, GameSave> DecompressAndDecode(string compressedAndEncoded)
		{
			if(compressedAndEncoded == String.Empty)
			{
				return new Dictionary<string, GameSave>();
			}
			byte[] compressed = _byteEncoder.ByteDecode(compressedAndEncoded);
			string json = Encoding.UTF8.GetString(_saveDataCompression.Decompress(compressed));
			return JsonConvert.DeserializeObject<Dictionary<string, GameSave>>(json);
		}
	}
}
