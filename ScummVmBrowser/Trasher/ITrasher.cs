using System;
using System.Threading.Tasks;

namespace Trasher
{
    public interface ITrasher: IDisposable
    {
        bool IsTrashed { get; }
        Task PauseTrashCountDown();
        Task ResumeTrashCountDown();
    }
}