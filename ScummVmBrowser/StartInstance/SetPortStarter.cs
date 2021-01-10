using ManagedCommon.Delegates;
using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartInstance
{
    public class SetPortStarter : IStarter
    {
        public int Port { get; set; }

        public bool StartConnection(StartConnection startConnection)
        {
          return startConnection(Port);
        }
    }
}
