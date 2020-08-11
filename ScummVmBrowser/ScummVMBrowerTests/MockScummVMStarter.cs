using ManagedCommon.Interfaces;
using ScummVMBrowser.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowserTests
{
    public class MockScummVMStarter : IScummVMServerStarter
    {
        public string PathToExecutableToStart => "C:\\test\\test.exe";

        public bool StartScummVM(string pathToExecutableToStart, string rpcPortGetterId, string realTimePortGetterId)
        {
            return true;
        }
    }
}
