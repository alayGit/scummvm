using ManagedCommon.Enums;
using ManagedCommon.ExtensionMethods;
using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Base
{
   public class SharedMemoryBase
    {
        private string _id;

        public IConfigurationStore<System.Enum> ConfigurationStore { get; private set; }

        public SharedMemoryBase(IConfigurationStore<System.Enum> configurationStore)
        {
            ConfigurationStore = configurationStore;
        }

        public virtual Task Init(string id)
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

            return rpcBufferNames.AsBufferName(_id);
        }
    }
}
