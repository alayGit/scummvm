using Ice;
using IceRpc.Base;
using IceRpc.DispatchInterceptor;
using IceRpc.I;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ManagedCommon.Interfaces.Rpc;
using ScummVMSlices;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc
{
    public class ScummVmIceClient : IceProxyClient<ScummHubPrx, IScummHubClientAsyncRpc>, IScummHubRpcAsyncProxy
    {
        private ILogger _logger;
        
        public ScummVmIceClient(ILogger logger)
        {
            _logger = logger;
        }

        public async Task RunGameAsync(AvailableGames gameName, string compressedAndEncodedGameSaveData)
        {
           await Sender.RunGameAsync(gameName.ToString(), compressedAndEncodedGameSaveData);
        }

        public void Quit()
        {
            Sender.Quit();
        }

        protected override ScummHubPrx GetSender(string host, int port, Protocol protocol)
        {
            ObjectPrx obj = Communicator.stringToProxy(GetProxyStringProxy(HubName, host, port, protocol));

            ScummHubPrx sender = ScummHubPrxHelper.checkedCast(obj);
            
            return sender;
        }

        protected override Ice.Object CreateNewServant()
        {
          return new ScummWebLoggingClientInterceptor(new ScummHubClientI(Receiver), _logger);
        }

        public override void Init(string hubName, string clientCallbackProxyName, string host, int port, string id, IScummHubClientAsyncRpc receiver, Protocol protocol)
        {
            base.Init(hubName, clientCallbackProxyName, host, port, id, receiver, protocol);
            Sender.addClient(ScummHubClientPrxHelper.uncheckedCast(ClientProxy));
        }
    }
}
