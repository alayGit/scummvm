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
        Task Init(string gameId);
        Task Quit();
        Task SendGameMessages(string connectionId, string gameMessages);
		Task EnqueueInputControls(byte[] compressedInputMessages);
	}
}
