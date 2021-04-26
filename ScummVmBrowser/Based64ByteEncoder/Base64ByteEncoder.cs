using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base64ByteEncoder
{
	public class Base64ByteEncoder : ISaveDataEncoder
	{
		public Encoding TextEncoding => Encoding.UTF8;

		public byte[] ByteDecode(string input)
		{
			return Convert.FromBase64String(input);
		}

		public string ByteEncode(byte[] input)
		{
			return Convert.ToBase64String(input);
		}
	}
}
