using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowser.Server
{
    public interface IHubServer : IDisposable
    {
        Task EnqueueControlKey(ControlKeys controlKey);
        Task EnqueueString(string toSend);
        Task Init(string gameId);
        Task Quit();
        Task NextFrame(string connectionId, List<KeyValuePair<MessageType, string>> screenBuffers);
    }
}
