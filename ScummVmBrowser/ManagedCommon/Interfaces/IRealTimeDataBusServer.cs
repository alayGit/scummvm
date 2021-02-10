using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
    public interface IRealTimeDataBusServer: IDisposable
    {
        Task DisplayFrameAsync(List<KeyValuePair<MessageType, string>> screenBuffers);
        Task Init(string id);
    }
}
