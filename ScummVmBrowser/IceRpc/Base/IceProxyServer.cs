using Ice;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using PortSharer;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc.Base
{
    public abstract class IceProxyServer<S,R>: IceProxy<S,R>, IRpcServerService<R> where S : class, ObjectPrx
    {
        private IStarter _starter;
        private IPortSender _portSender;
  
        protected abstract S GetSender(string id);

        public IceProxyServer(IStarter starter, IPortSender portSender)
        {
            _starter = starter;
            _portSender = portSender;
        }

        public void Init(string hubName, string clientCallbackProxyName, string host, string rpcPortGetterId, R receiver, Protocol protocol)
        {
          if(!_starter.StartConnection(p =>
            {
                base.Init(hubName, clientCallbackProxyName, host, p, rpcPortGetterId, receiver, protocol);
                ObjectAdapter.add(Servant, Ice.Util.stringToIdentity(HubName));
                ObjectAdapter.activate();
                
                return true;
            }))
            {
                throw new System.Exception("Failed to start");
            }
            _portSender?.Init(() => Port, rpcPortGetterId);
        }

        protected override ObjectAdapter GetObjectAdapter(string host, int port, Protocol protocol)
        {
            return Communicator.createObjectAdapterWithEndpoints(HubName, GetProxyStringObjAdaptor(host, port, protocol));
            //if (proxyType == ProxyType.Server)
            //{
            //    _objectAdapter = Communicator.createObjectAdapterWithEndpoints(HubName, GetProxyStringObjAdaptor(host, port));
            //}
            //else
            //{
            //    _objectAdapter = Communicator.createObjectAdapter("");
            //}
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(disposing)
            {
                _portSender?.Dispose();
            }
        }
    }
}
