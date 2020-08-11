using ManagedCommon.Interfaces;
using ScummVMBrowser.Clients;
using ScummVMBrowser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trasher;

namespace TestMocks
{
    public class MockGameInfo : IGameInfo
    {
        public IScummVMHubClient Client { get; set; }

        public MockGameInfo(IScummVMHubClient client)
        {
            Client = client;
        }

        public IEnumerable<Func<Task>> WhenTrashedSubscribers => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IAntiDisposalLock<IScummVMHubClient>> GetClient()
        {
            IAntiDisposalLock<IScummVMHubClient> result = new MockAntiDisposalLock<IScummVMHubClient>(Client);
            return Task.FromResult(result);
        }

        public void SubscribeToTrashEvent(Func<Task> onTrashed)
        {
            throw new NotImplementedException();
        }
    }
}
