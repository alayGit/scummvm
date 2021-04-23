using Ice;
using IceInternal;
using IceRpc.Base;
using IceRpc.I;
using ManagedCommon.Interfaces;
using ManagedCommon.Interfaces.Rpc;
using PortSharer;
using ScummVMSlices;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedCommon.Enums;
using System.Diagnostics;
using IceRpc.DispatchInterceptor;

namespace IceRpc
{
    public class ScummVmIceServer : IceProxyServer<ScummHubClientPrx, IScummHubRpc>, IScummHubClientRpcProxy
    {
        private ScummHubClientPrx _sender;
        private ScummVmLoggingServerInterceptor _scummVmLoggingInterceptor;

        private ILogger _logger;

        private ScummHubClientPrx Sender
        {
            get {

                if (_sender == null)
                {
                    _sender = GetSender(string.Empty);
                }

                return _sender;
            }
        }

        public ScummVmIceServer(IStarter starter, IPortSender portSender, ILogger logger):base(starter, portSender)
        {
            _logger = logger;
        }

        public bool SaveGame(string saveData)
        {
            return Sender.SaveGame(saveData);
        }

        protected override ScummHubClientPrx GetSender(string id)
        {
            return _scummVmLoggingInterceptor.Servant.ClientPrx;
        }
        
        protected override Ice.Object CreateNewServant()
        {
            _scummVmLoggingInterceptor = new ScummVmLoggingServerInterceptor(new ScummHubI(Receiver), _logger);
            return _scummVmLoggingInterceptor;
        }

        public void BackEndQuit()
        {
            Sender.BackEndQuit();
        }
    }
}
