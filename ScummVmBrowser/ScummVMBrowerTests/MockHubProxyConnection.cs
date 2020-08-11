using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SignalRHostWithUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowerTests
{
    public class MockHubProxyConnection : IHubProxyConnection
    {

        public Version Protocol { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan TransportConnectTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TimeSpan TotalTransportConnectTimeout => throw new NotImplementedException();

        public TimeSpan ReconnectWindow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public KeepAliveData KeepAliveData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string MessageId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string GroupsToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IDictionary<string, object> Items => throw new NotImplementedException();

        public string ConnectionId => throw new NotImplementedException();

        public string ConnectionToken => throw new NotImplementedException();

        public string Url => throw new NotImplementedException();

        public string QueryString => throw new NotImplementedException();

        public ConnectionState State => throw new NotImplementedException();

        public IClientTransport Transport => throw new NotImplementedException();

        public DateTime LastMessageAt => throw new NotImplementedException();

        public DateTime LastActiveAt => throw new NotImplementedException();

        public IWebProxy Proxy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public X509CertificateCollection Certificates => throw new NotImplementedException();

        public IDictionary<string, string> Headers => throw new NotImplementedException();

        public ICredentials Credentials { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CookieContainer CookieContainer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public JsonSerializer JsonSerializer => throw new NotImplementedException();

        public IHubProxy HubProxy { get; set; }

        public bool IsDisposed { get; set; }

        public MockHubProxyConnection(IHubProxy hubProxy)
        {
            HubProxy = hubProxy;
            IsDisposed = false;
        }

        public bool ChangeState(ConnectionState oldState, ConnectionState newState)
        {
            throw new NotImplementedException();
        }

        public IHubProxy CreateHubProxy(string hubName)
        {
            return HubProxy;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public void MarkActive()
        {
            throw new NotImplementedException();
        }

        public void MarkLastMessage()
        {
            throw new NotImplementedException();
        }

        public void OnConnectionSlow()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void OnReceived(JToken data)
        {
            throw new NotImplementedException();
        }

        public void OnReconnected()
        {
            throw new NotImplementedException();
        }

        public void OnReconnecting()
        {
            throw new NotImplementedException();
        }

        public void PrepareRequest(IRequest request)
        {
            throw new NotImplementedException();
        }

        public Task Send(string data)
        {
            throw new NotImplementedException();
        }

        public Task Start()
        {
            return Task.CompletedTask;  
        }

        public void Stop()
        {

        }

        public void Stop(Exception error)
        {
            throw new NotImplementedException();
        }

        public void Trace(TraceLevels level, string format, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
