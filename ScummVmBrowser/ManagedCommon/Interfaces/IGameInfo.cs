using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ManagedCommon.Interfaces
{
    public interface IGameInfo: ITrashable
    {
        Task<IAntiDisposalLock<IScummVMHubClient>> GetClient();
    }
}