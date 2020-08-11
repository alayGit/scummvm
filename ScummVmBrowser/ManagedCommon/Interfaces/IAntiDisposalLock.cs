using System;

namespace ManagedCommon.Interfaces
{
    public interface IAntiDisposalLock<T>: IDisposable where T : IDisposable
    {
        T Obj { get; }
    }
}