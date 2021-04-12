using ManagedCommon.Delegates;
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

		public SaveDataEncoderAndCompressor(IByteEncoder byteEncoder, ISaveDataCompression saveDataCompression)
		{
			_byteEncoder = byteEncoder;
			_saveDataCompression = saveDataCompression;
		}

		public string CompressAndEncode(IDictionary<string, GameSave> saveData)
		{
			string json = JsonConvert.SerializeObject(saveData);
			byte[] compressed = _saveDataCompression.Compress(Encoding.UTF8.GetBytes(json), 10); //TODO: Add in compression level
			return _byteEncoder.ByteEncode(compressed);
		}

		public IDictionary<string, GameSave> Decompress(string compressedAndEncoded)
		{
			byte[] compressed = _byteEncoder.ByteDecode(compressedAndEncoded);
			string json = Encoding.UTF8.GetString(_saveDataCompression.Decompress(compressed));
			return JsonConvert.DeserializeObject<Dictionary<string, GameSave>>(json);
		}
	}
}
