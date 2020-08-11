using IceRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestIceWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(30000);
            ScummVmIceWebServerClient scummVmIceClient = new ScummVmIceWebServerClient();

            scummVmIceClient.Init("ScummWebServerHub", "ScummWebServerHub", "localhost", 5632, Guid.NewGuid().ToString(), null, ManagedCommon.Enums.Protocol.ws);

            //scummVmIceClient.RunGame(ManagedCommon.Enums.AvailableGames.kq3, null);
        }
    }
}
