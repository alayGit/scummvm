using ManagedCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
    public interface IRealTimeDataBusServer: IDisposable
    {
        Task DisplayFrameAsync(List<ScreenBuffer> screenBuffers);
        Task PlaySound(byte[] data);
        Task Init(string id);

    }
}
