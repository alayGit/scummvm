using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Enums
{
    public enum RpcBufferNames
    {
        DisplayFrame,
        PlaySound,
        StartSound,
        StopSound,
        EnqueueString,
        EnqueueControlKey,
        EnqueueMouseMove,
        EnqueueMouseClick,
        GetWholeScreenBuffer,
    }
}