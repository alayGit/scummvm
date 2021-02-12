using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using ManagedCommon.Interfaces;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScummVMBrowser.Server
{
	public class HubServer : Hub, IHubServer
    {
        private IGameClientStore<IGameInfo> HubStore { get; set; }
        private ILogger _logger;

        public HubServer(IGameClientStore<IGameInfo> hubStore, ILogger logger)
        {
            HubStore = hubStore;
            _logger = logger;
        }

        private string ConnectionId
        {
            get
            {
                return Context.ConnectionId;
            }
        }

        private IGameInfo GameInfo
        {
            get
            {
                return HubStore.GetByConnectionId(ConnectionId);
            }
        }

        public async Task Init(string gameId)
        {
            HubStore.AssociateConnectionWithGame(ConnectionId, gameId);

            using (IAntiDisposalLock<IScummVMHubClient> alock = await HubStore.GetByConnectionId(ConnectionId)?.GetClient())
            {
                IScummVMHubClient client = alock.Obj;

                if (client != null)
                {
                    client.SetSendGameMessagesFunctionPointer((List<KeyValuePair<MessageType, string>> screenBuffers) => SendGameMessages(ConnectionId, screenBuffers));
                }
            }
        }

        public async Task EnqueueString(string toSend)
        {
            using (IAntiDisposalLock<IScummVMHubClient> alock = await HubStore.GetByConnectionId(ConnectionId)?.GetClient())
            {
                await alock?.Obj.EnqueueString(toSend);
            }
        }

        public async Task EnqueueControlKey(ControlKeys controlKey)
        {
            using (IAntiDisposalLock<IScummVMHubClient> alock = await HubStore.GetByConnectionId(ConnectionId)?.GetClient())
            {
                await alock.Obj?.EnqueueControlKey(controlKey);
            }
        }

        public async Task EnqueueMouseMove(int x, int y)
        {
            using (IAntiDisposalLock<IScummVMHubClient> alock = await HubStore.GetByConnectionId(ConnectionId)?.GetClient())
            {
                await alock.Obj?.EnqueueMouseMove(x, y);
            }
        }

        public async Task EnqueueMouseClick(MouseClick mouseClick)
        {
            using (IAntiDisposalLock<IScummVMHubClient> alock = await HubStore.GetByConnectionId(ConnectionId)?.GetClient())
            {
                await alock.Obj?.EnqueueMouseClick(mouseClick);
            }
        }

        public async Task Quit()
        {
            using (IAntiDisposalLock<IScummVMHubClient> alock = await HubStore.GetByConnectionId(ConnectionId)?.GetClient())
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

        public async Task StartSound()
        {
            using (IAntiDisposalLock<IScummVMHubClient> alock = await HubStore.GetByConnectionId(ConnectionId)?.GetClient())
            {
                IScummVMHubClient client = alock?.Obj;

                await client.StartSound();
            }
        }

        public async Task StopSound()
        {
            using (IAntiDisposalLock<IScummVMHubClient> alock = await HubStore.GetByConnectionId(ConnectionId)?.GetClient())
            {
                IScummVMHubClient client = alock?.Obj;

                await client.StopSound();
            }
        }

        public async Task SendGameMessages(string connectionId, List<KeyValuePair<MessageType, string>> gameMessages)
        {
            await Clients.Client(connectionId).SendGameMessages(gameMessages);
        }

        private void SaveGame(string connectionId, IEnumerable<byte> saveData, String saveName)
        {
            Clients.Client(connectionId).SaveData(saveData, saveName);
        }
    }
}
