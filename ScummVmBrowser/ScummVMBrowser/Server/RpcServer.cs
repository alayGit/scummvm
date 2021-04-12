using Antlr.Runtime.Tree;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ManagedCommon.Interfaces.Rpc;
using Microsoft.AspNet.SignalR.Configuration;
using ScummVMBrowser.Data;
using ScummVMBrowser.Models;
using ScummVMBrowser.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Trasher;

namespace ScummVMBrowser.Server
{
    public class RpcServer : IScummWebServerRpc
    {
        private IScummWebServerClientRpcProxy _scummWebServerClientRpcProxy;
        IConfigurationStore<Enum> _configurationStore;
        private IGameClientStore<IGameInfo> _gameClientStore;

        public RpcServer(IScummWebServerClientRpcProxy scummWebServerClientRpcProxy, IConfigurationStore<System.Enum> configurationStore, IGameClientStore<IGameInfo> gameClientStore)
        {
            _configurationStore = configurationStore;
            _gameClientStore = gameClientStore;
            _scummWebServerClientRpcProxy = scummWebServerClientRpcProxy;
            _scummWebServerClientRpcProxy.Init(_configurationStore.GetValue(IceRemoteProcFrontEnd.HubName), _configurationStore.GetValue(IceRemoteProcFrontEnd.ServerName), _configurationStore.GetValue(IceRemoteProcFrontEnd.HostName), Guid.NewGuid().ToString(), this, Protocol.ws);
        }

        public string GetControlKeys()
        {
            return EnumExtensions.EnumToString<ControlKeys>();
        }

        public async Task InitAsync(string gameId)
        {
            IGameInfo gameInfo = _gameClientStore.GetByGameId(gameId);

            if (gameInfo == null)
            {
                _gameClientStore.Store(gameId);
                gameInfo = _gameClientStore.GetByGameId(gameId);
            }

            using (IAntiDisposalLock<IScummVMHubClient> alock = await gameInfo?.GetClient())
            {
                IScummVMHubClient client = alock.Obj;
                
                if (!client.IsGameRunning)
                {
                    await client.Init();
                    client.SetSaveGameFunctionPointer(async (string saveData, string saveName) => await _scummWebServerClientRpcProxy.SaveGameAsync(saveData, saveName, gameId));
                }
            }
        }

        public async Task RunGameAsync(AvailableGames gameName, string signalrConnectionId, string saveStorage)
        {
            using (IAntiDisposalLock<IScummVMHubClient> alock = await _gameClientStore.GetByConnectionId(signalrConnectionId)?.GetClient())
            {
                IScummVMHubClient client = alock.Obj;

                if (client != null)
                {
                    if (!client.IsGameRunning)
                    {
                        await client.StartConnection();
                        await client.StartGame(saveStorage, gameName);
                    }
                    await client.ScheduleRedrawWholeScreen();
                }
            }
        }

        public async Task QuitAsync(string signalRConnectionId)
        {
            using (IAntiDisposalLock<IScummVMHubClient> alock = await _gameClientStore.GetByConnectionId(signalRConnectionId)?.GetClient())
            {
                IScummVMHubClient client = alock?.Obj;

                if (client != null && client.IsGameRunning)
                {
                    await client.Quit();
                }
                else
                {
                    throw new Exception("Cannot quit while game is not running");
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _scummWebServerClientRpcProxy.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
