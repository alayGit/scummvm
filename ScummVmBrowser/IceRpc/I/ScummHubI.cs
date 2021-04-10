using Ice;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ScummVMSlices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc.I
{
    public class ScummHubI : ScummHubDisp_
    {
        private IScummHubRpc _receiver;
        
        public ScummHubClientPrx ClientPrx { get; private set; }

        public ScummHubI(IScummHubRpc server)
        {
            _receiver = server;
        }

        public async override Task RunGameAsync(string gameName, string gameStorage, Current current = null)
        {
          await _receiver.RunGameAsync((AvailableGames)Enum.Parse(typeof(AvailableGames), gameName), gameStorage);
        }

        public override void Quit(Current current = null)
        {
            _receiver.Quit();
        }

        public override void addClient(ScummHubClientPrx receiver, Current current = null)
        {
            ClientPrx = (ScummHubClientPrx) receiver.ice_fixed(current.con);
        }
    }
}
