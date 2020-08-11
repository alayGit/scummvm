using ManagedCommon.Enums;
using ManagedCommon.ExtensionMethods;
using ManagedCommon.Interfaces;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Base
{
    public class HubBase: Hub
    {
        private string _id;
        protected string CurrentConnectionId { get; set; }

        public IConfigurationStore<System.Enum> ConfigurationStore { get; private set; }

        public HubBase(IConfigurationStore<System.Enum> configurationStore)
        {
            ConfigurationStore = configurationStore;
        }

        public Task Init(string id)
        {
            _id = id;
            return Task.CompletedTask;
        }

        protected string GetId(RpcBufferNames rpcBufferNames)
        {
            if (_id == null)
            {
                throw new Exception("Failed to call init to set id");
            }

          return _id;
        }

        public async override Task OnConnected()
        {
            CurrentConnectionId = Context.ConnectionId;

            await base.OnConnected();
        }
    }
}
