using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Models
{
	public class GameSave
	{
		public byte[] Data { get; set; }
		public byte[] Thumbnail { get; set; }
		public string PaletteString { get; set; }
	}
}
