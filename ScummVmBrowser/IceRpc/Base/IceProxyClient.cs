using Ice;
using IceRpc.Interfaces;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc.Base
{
    public abstract class IceProxyClient<S, R> : IceProxy<S, R>, IRpcClientService<R> where S : class, ObjectPrx
    {
        protected ObjectPrx ClientProxy { get; private set; }

        private S _sender;


        protected S Sender
        {
            get
            {
                if (_sender == null)
                {
                    _sender = GetSender(Host, Port, Protocol);

                    if (_sender == null)
                    {
                        throw new ApplicationException("Invalid proxy");
                    }
                }

                return _sender;
            }
        }

        protected abstract S GetSender(string host, int port, Protocol protocol);


        protected override ObjectAdapter GetObjectAdapter(string host, int port, Protocol protocol)
        {
            return Communicator.createObjectAdapter(String.Empty);
        }

        public override void Init(string hubName, string clientCallbackProxyName, string host, int port, string id, R receiver, Protocol protocol)
        {
            base.Init(hubName, clientCallbackProxyName, host, port, id, receiver, protocol);

            if (Servant != null)
            {
                ClientProxy = ObjectAdapter.add(Servant, Ice.Util.stringToIdentity(ClientCallbackProxyName));
                ObjectAdapter.activate();
                Sender.ice_getConnection().setAdapter(ObjectAdapter);
            }
        }
    }
}
