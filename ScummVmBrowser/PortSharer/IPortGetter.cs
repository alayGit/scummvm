using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortSharer
{
    public interface IPortGetter: IDisposable
    {
        Task<int> GetPort(string id);
    }
}
