using Ice;
using ManagedCommon.Interfaces.Rpc;
using ScummVMSlices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc.I
{
    public class ScummHubClientI : ScummHubClientDisp_
    {
        private IScummHubClientAsyncRpc _receiver;

        public ScummHubClientI(IScummHubClientAsyncRpc client)
        {
            _receiver = client;
        }

        public override Task BackEndQuitAsync(Current current = null)
        {
             return _receiver.BackEndQuitAsync();
        }

        public override Task<bool> SaveGameAsync(string saveData, string fileName, Current current = null)
        {
            return _receiver.SaveGameAsync(saveData, fileName);
        }
    }
}
