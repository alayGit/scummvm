using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using ManagedCommon.Interfaces;
using Microsoft.AspNet.SignalR;
using ScummVMBrowser.Clients;
using ScummVMBrowser.Data;
using ScummVMBrowser.Models;
using ScummVMBrowser.StaticData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Trasher;
using Unity;

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
                    client.SetNextFrameFunctionPointer((List<List<ScreenBuffer>> screenBuffers) => NextFrame(ConnectionId, screenBuffers));
                    client.SetPlaySoundFunctionPointer((byte[] data) => PlaySound(ConnectionId, data));
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

        public async Task NextFrame(string connectionId, List<List<ScreenBuffer>> screenBuffers)
        {
            await Clients.Client(connectionId).NextFrame(screenBuffers);
        }

        private void SaveGame(string connectionId, IEnumerable<byte> saveData, String saveName)
        {
            Clients.Client(connectionId).SaveData(saveData, saveName);
        }

        public Task PlaySound(string connectionId, byte[] data)
        {
            return Clients.Client(connectionId).PlaySound(data);
        }

        //public async override Task OnDisconnected(bool stopCalled)
        //{
        //    await Quit();
        //    await base.OnDisconnected(stopCalled);
        //}
    }
}
