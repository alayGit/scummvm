using Ice;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc
{
    public abstract class IceProxy<S,R> where S: class, ObjectPrx
    {
        
        protected Communicator Communicator { get; private set; }

        private Ice.Object _servant;
        protected Ice.Object Servant {
            get { 
                if(_servant == null)
                {
                    _servant = CreateNewServant();
                }
                return _servant;
            }
        }
        
        public R Receiver { get; set; }

        protected ObjectAdapter ObjectAdapter { get; private set; }

        protected Protocol Protocol { get; set; }

        public string HubName { get; private set; }
        public string ClientCallbackProxyName { get; private set; }

        public string Host { get; private set; }

        public int Port { get; private set; }

        protected abstract Ice.Object CreateNewServant();


        public IceProxy()
        {
			InitializationData initializationData = new InitializationData();
			Ice.Properties props = Ice.Util.createProperties();

			props.setProperty("Ice.ACM.Heartbeat", "3");
			Ice.InitializationData initData = new Ice.InitializationData();
			initData.properties = props;

			Communicator = Ice.Util.initialize(initData);
        }

        public virtual void Init(string hubName, string clientCallbackProxyName, string host, int port, string id, R receiver, Protocol protocol)
        {
            Host = host;
            Port = port;
            Protocol = protocol;
            Receiver = receiver;
            HubName  = $"{hubName}{Port.ToString()}";
            ClientCallbackProxyName = $"{clientCallbackProxyName}{Port.ToString()}";
            ObjectAdapter = GetObjectAdapter(host, Port, protocol);
        }

        protected abstract ObjectAdapter GetObjectAdapter(string host, int port, Protocol protocol);

        protected string GetProxyStringObjAdaptor(string host, int port, Protocol protocol)
        {
            return $"{protocol.ToString()} -h {host} -p {port.ToString()}";
        }

        protected string GetProxyStringProxy(string hubName, string host, int port, Protocol protocol)
        {
            return $"{hubName}:{protocol.ToString()} -h {host} -p {port.ToString()}";
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Communicator.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
