using ManagedCommon.Interfaces;
using ScummVMBrowser.StaticData;
using StartInstance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ScummVMBrowser.Clients
{
    public class ScummVMServerStarter : IScummVMServerStarter
    {
        public bool StartScummVM(string pathToExecutableToStart, string rpcPortGetterId, string realTimePortGetterId)
        {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = pathToExecutableToStart;
                myProcess.StartInfo.CreateNoWindow = false;
                myProcess.StartInfo.Arguments = $"{rpcPortGetterId} {realTimePortGetterId}";

                return myProcess.Start();
            }
        }
    }
}
