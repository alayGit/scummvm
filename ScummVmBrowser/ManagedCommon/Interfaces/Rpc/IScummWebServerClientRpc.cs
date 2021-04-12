using ManagedCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces.Rpc
{
    public interface IScummWebServerClientRpc
    {
        Task<bool> SaveGameAsync(string saveData, string fileName, string id);
    }
}
