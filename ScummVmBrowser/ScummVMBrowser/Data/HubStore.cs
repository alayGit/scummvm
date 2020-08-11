using ManagedCommon.Interfaces;
using ManagedCommon.Interfaces.Rpc;
using ScummVMBrowser.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Trasher;
using Unity;

namespace ScummVMBrowser.Data
{

    public class HubStore<T> : IGameClientStore<T> where T : class, ITrashable
    {
        private ConcurrentDictionary<string, T> DictByGameId { get; set; }
        private ConcurrentDictionary<string, string> DictByConnectionString { get; set; }
        private IUnityContainer Container { get; set; }
        private IRpcClientProxyRemover<string> RpcClientProxyRemover { get; set; }

        public HubStore(IUnityContainer unityContainer, IRpcClientProxyRemover<string> rpcClientProxyRemover)
        {
            DictByGameId = new ConcurrentDictionary<string, T>();
            DictByConnectionString = new ConcurrentDictionary<string, string>();
            Container = unityContainer;
            RpcClientProxyRemover = rpcClientProxyRemover;
        }

        public T GetByConnectionId(string connectionId)
        {
            string gameId;
            T info = null;

            DictByConnectionString.TryGetValue(connectionId, out gameId);

            if (gameId != null)
            {
                DictByGameId.TryGetValue(gameId, out info);
            }

            return info;
        }

        public T GetByGameId(string gameId)
        {
            T info;

            DictByGameId.TryGetValue(gameId, out info);

            return info;
        }

        public void AssociateConnectionWithGame(string connectionId, string gameId)
        {
            T info = GetByGameId(gameId);
            DictByConnectionString[connectionId] = gameId;

            //TODO: Subscribes every time BAD
            info.SubscribeToTrashEvent(
                  async () =>
                  {
                      DictByConnectionString.TryRemove(connectionId, out _);
                      await Task.CompletedTask;
                  }
               );
        }

        public void Store(string gameId)
        {
            T info = Container.Resolve<T>();
            if (DictByGameId.ContainsKey(gameId))
            {
                throw new Exception($"GameId {gameId} already exists in the dictionary");
            }
            DictByGameId[gameId] = info;

            info.SubscribeToTrashEvent(
                   async () =>
                    {
                        DictByGameId.TryRemove(gameId, out _);
                        await Task.CompletedTask;
                        RpcClientProxyRemover.Remove(gameId);
                    }
                );
        }

        public string GetGameIdByConnectionId(string connectionId)
        {
            return DictByConnectionString[connectionId];
        }
    }
}