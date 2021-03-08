using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using ManagedCommon.Enums.Other;
using ManagedCommon.Interfaces;
using ManagedCommon.Models;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using ScummVMBrowser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowser.Server
{
	public class HubServer : Hub, IHubServer
    {
        private IGameClientStore<IGameInfo> HubStore { get; set; }
        private ILogger _logger;
		private IInputMessageProcessor _inputMessageProcessor;
		private ICompression _compression;

        public HubServer(IGameClientStore<IGameInfo> hubStore, ILogger logger, ICompression compression)
        {
            HubStore = hubStore;
            _logger = logger;
			_compression = compression;
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
                    client.SetSendGameMessagesFunctionPointer((string gameMessages) => SendGameMessages(ConnectionId, gameMessages));
                }
            }
        }

		public async Task EnqueueInputControls(byte[] compressedInputMessages)
		{
            using (IAntiDisposalLock<IScummVMHubClient> alock = await HubStore.GetByConnectionId(ConnectionId)?.GetClient())
            {
                await alock.Obj?.EnqueueInputMessages(GetInputMessagesFromCompressed(compressedInputMessages));
            }
        }

		private KeyValuePair<string,string>[] GetInputMessagesFromCompressed(byte[] compressedInputMessages)
		{
			string jsonMessage = Encoding.ASCII.GetString(_compression.Decompress(compressedInputMessages));

			return JsonConvert.DeserializeObject<KeyValuePair<string, string>[]>(jsonMessage);
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

        public async Task SendGameMessages(string connectionId, string gameMessages)
        {
            await Clients.Client(connectionId).SendGameMessages(gameMessages);
        }

        private void SaveGame(string connectionId, IEnumerable<byte> saveData, String saveName)
        {
            Clients.Client(connectionId).SaveData(saveData, saveName);
        }
    }
}
