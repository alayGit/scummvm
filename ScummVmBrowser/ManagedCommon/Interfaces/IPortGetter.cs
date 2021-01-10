using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
    public interface IPortGetter: IDisposable
    {
        Task<int> GetPort(string id);
    }
}
