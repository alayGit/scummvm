using IceInternal;
using IceRpc.Base;
using IceRpc.DispatchInterceptor;
using IceRpc.I;
using ManagedCommon.Interfaces;
using ManagedCommon.Interfaces.Rpc;
using PortSharer;
using ScummWebsServerVMSlices;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc.IceServerImplementations
{
    public class ScummWebIceServer : IceProxyServer<ScummWebServerClientPrx, IScummWebServerRpc>, IScummWebServerClientRpcProxy
    {
        private IReadOnlyDictionary<string, ScummWebServerClientPrx> _rpcClientProxies;

        private ILogger _logger;

        public ScummWebIceServer(IStarter starter, IPortSender portSender, IReadOnlyDictionary<string, ScummWebServerClientPrx> rpcClientProxies, ILogger logger): base(starter, portSender)
        {
            _rpcClientProxies = rpcClientProxies;
            _logger = logger;
        }

        public Task<bool> SaveGameAsync(string saveData, string fileName, string id)
        {
            return GetSender(id).SaveGameAsync(saveData, fileName);
        }

       

        protected override Ice.Object CreateNewServant()
        {
            return new ScummWebLoggingServerInterceptor(new ScummWebServerI(Receiver), _logger);
        }

        protected override ScummWebServerClientPrx GetSender(string id)
        {
          return  _rpcClientProxies[id];
        }

    }
}
