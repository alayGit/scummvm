using ManagedCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
	public interface ISaveCache
	{
		void SaveToCache(string name, GameSave saveData);
		void RemoveFromCache(string name);
		IEnumerable<string> ListCache();
		void SetCache(string yEncodedCompressedCache);
		string GetCompressedAndEncodedSaveData();
		byte[] GetFromCache(string name);

		int SaveCount { get; }
	}
}
