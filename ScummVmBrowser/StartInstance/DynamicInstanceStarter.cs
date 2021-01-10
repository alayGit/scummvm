using ManagedCommon.Delegates;
using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace StartInstance
{
    public class DynamicInstanceStarter : IStarter
    {
        public const int NoTrys = 10; //ToDo: Configure this
        public const int StartPort = 8080;

        public delegate bool StartProcess(int port);


        public bool StartConnection(StartConnection startConnection)
        {
            bool result = false;
            for (int startAttempt = 1; startAttempt <= NoTrys && !result; startAttempt++)
            {
                int possiblePort;
                try
                {
                    possiblePort = GetAvailablePort(StartPort);
                    result = startConnection(possiblePort);
                }
                catch (Exception e)
                {
                    if (startAttempt == NoTrys)
                    {
                        throw e;
                    }
                }
            }
            return result;
        }

        //Got this from: https://gist.github.com/jrusbatch/4211535
        private static int GetAvailablePort(int startingPort)
        {
            IPEndPoint[] endPoints;
            List<int> portArray = new List<int>();

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

            //getting active connections
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            portArray.AddRange(from n in connections
                               where n.LocalEndPoint.Port >= startingPort
                               select n.LocalEndPoint.Port);

            //getting active tcp listners - WCF service listening in tcp
            endPoints = properties.GetActiveTcpListeners();
            portArray.AddRange(from n in endPoints
                               where n.Port >= startingPort
                               select n.Port);

            //getting active udp listeners
            endPoints = properties.GetActiveUdpListeners();
            portArray.AddRange(from n in endPoints
                               where n.Port >= startingPort
                               select n.Port);

            portArray.Sort();

            for (int i = startingPort; i < UInt16.MaxValue; i++)
                if (!portArray.Contains(i))
                    return i;

            return 0;
        }
    }
}
