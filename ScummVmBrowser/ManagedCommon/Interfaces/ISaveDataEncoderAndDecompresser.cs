using ManagedCommon.Delegates;
using ManagedCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
	public interface ISaveDataEncoderAndDecompresser
	{
		string CompressAndEncode(IDictionary<string, GameSave> saveData);
		IDictionary<string, GameSave> Decompress(string compressedAndEncoded);
	}
}
