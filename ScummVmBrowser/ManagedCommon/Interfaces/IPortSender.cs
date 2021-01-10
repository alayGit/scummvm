using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
    public delegate int GetPort();
   public interface IPortSender: IDisposable
    {
        void Init(GetPort getPort, string id);
    }
}
