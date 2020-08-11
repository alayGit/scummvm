using Ice;
using IceRpc.Base;
using IceRpc.I;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ManagedCommon.Interfaces.Rpc;
using ScummVMSlices;
using ScummWebsServerVMSlices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc
{
    public class ScummVmIceWebServerClient : IceProxyClient<ScummWebServerPrx, IScummWebServerClientRpc>, IScummWebServerRpcProxy
    {
 
        public void RunGame(AvailableGames gameName, Dictionary<string, byte[]> gameStorage)
        {
            Sender.Init(gameName.ToString(), "", "", null);
        }

        public void Quit()
        {
        }

        protected override ScummWebServerPrx GetSender(string host, int port, Protocol protocol)
        {
            ObjectPrx obj = Communicator.stringToProxy(GetProxyStringProxy(HubName, host, port, protocol));

            ScummWebServerPrx sender = ScummWebServerPrxHelper.checkedCast(obj);
            
            return sender;
        }

        public override void Init(string hubName, string clientCallbackProxyName, string host, int port, string id, IScummWebServerClientRpc receiver, Protocol protocol)
        {
            base.Init(hubName, clientCallbackProxyName, host, port, id, receiver, protocol);
            //Sender.addClient(ScummHubClientPrxHelper.uncheckedCast(ClientProxy));
        }

        protected override Ice.Object CreateNewServant()
        {
            return null;
        }
    }
}
