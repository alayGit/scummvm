using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
	public interface ICompression
	{
		byte[] Compress(byte[] input);
		byte[] Decompress(byte[] input);
	}
}
