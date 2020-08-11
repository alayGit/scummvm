﻿using Ice;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces.Rpc;
using Newtonsoft.Json;
using ScummWebsServerVMSlices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity;

namespace IceRpc.I
{
    public class ScummWebServerI : ScummWebServerDisp_
    {
        public static IUnityContainer Container;
        private IScummWebServerRpc _receiver;
        private IDictionary<string, ScummWebServerClientPrx> _clientProxies;

        private IDictionary<string, ScummWebServerClientPrx> ClientProxies
        {
            get
            {
                if(_clientProxies == null)
                {
                    _clientProxies = Container.Resolve<IDictionary<string, ScummWebServerClientPrx>>();
                }

                return _clientProxies;
            }
        }

        public ScummWebServerI():base()
        {
            _clientProxies = new Dictionary<string, ScummWebServerClientPrx>();
        }

        public ScummWebServerI(IScummWebServerRpc receiver)
        {
            _receiver = receiver;
        }

        public override string GetControlKeys(Current current = null)
        {
            return _receiver.GetControlKeys();
        }

        public async override Task InitAsync(string gameId, Current current = null)
        {
            await _receiver.InitAsync(gameId);
        }

        public override void addClient(ScummWebServerClientPrx receiver, string gameId, Current current = null)
        {
            //if(ClientProxies.ContainsKey(gameId))
            //{
            //   ClientProxies[gameId].ice_router
            //} //TODO: Investigate disposal

            ClientProxies[gameId] = (ScummWebServerClientPrx)receiver.ice_fixed(current.con); 
        }

        public async override Task QuitAsync(string signalRConnectionId, Current current = null)
        {
            await _receiver.QuitAsync(signalRConnectionId);
        }

        public async override Task RunGameAsync(string gameName, string signalrConnectionId, string saveStorageStr, Current current = null)
        {
            Dictionary<string, string> base64Dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(saveStorageStr);
            Dictionary<string, byte[]> byteDict = base64Dict.Select(kvp => new KeyValuePair<string, byte[]>(kvp.Key, Convert.FromBase64String(kvp.Value))).ToDictionary(pair => pair.Key, pair => pair.Value);

            //Dictionary<string, string> saveDict = JsonConvert.DeserializeObject<Dictionary<string,string>(saveStorageStr);

            await _receiver.RunGameAsync((AvailableGames)Enum.Parse(typeof(AvailableGames),gameName), signalrConnectionId, byteDict);
        }
    }
}
