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
        string ByteEncode(byte[] input);
        byte[] ByteDecode(string input);

		Encoding TextEncoding { get; }
	}
}
