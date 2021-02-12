using ManagedCommon.Enums.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
   public interface IByteEncoder
    {
        string AssciiByteEncode(byte[] input);
        byte[] AssciiByteDecode(string input);

		string AssciiStringEncode(string input);
		string AssciiStringDecode(string input);
	}
}
