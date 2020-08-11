using ManagedCommon.Interfaces;
using ScummVMBrowser.Data;
using ScummVMBrowser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace ScummVMBrowerTests
{
    public class MockHubStore : IGameClientStore<IGameInfo>
    {
        public Dictionary<string, string> ConIdStore { get; set; }
        private IUnityContainer _container;

        public MockHubStore(IUnityContainer container)
        {
            ConIdStore = new Dictionary<string, string>();
            _container = container;
        }

        public void AssociateConnectionWithGame(string connectionId, string gameId)
        {
            if(!ConIdStore.ContainsKey(connectionId))
            {
                ConIdStore[connectionId] = gameId;
            }
            else
            {
                throw new Exception("Trying To Store The Same Thing Twice");
            }
        }

        public IGameInfo GetByConnectionId(string connectionId)
        {
            if (ConIdStore.ContainsKey(connectionId))
            {
                return _container.Resolve<IGameInfo>();
            }
            else
            {
                throw new Exception($"No game stored by id {connectionId}");
            }
        }

        public IGameInfo GetByGameId(string gameId)
        {
            throw new NotImplementedException();
        }

        public void Store(string gameId)
        {
            throw new NotImplementedException();
        }

        public string GetGameIdByConnectionId(string connectionId)
        {
            throw new NotImplementedException();
        }
    }
}
