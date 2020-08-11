﻿using ManagedCommon.Enums.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
   public interface IByteEncoder
    {
        string AssciiEncode(byte[] input);
        byte[] AssciiDecode(string input);
    }
}
