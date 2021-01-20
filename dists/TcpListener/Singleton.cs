using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	internal static class Singleton
	{
		private static Dictionary <int, TcpListenerThread> _listenerDictionary;

		static Singleton()
		{
			_listenerDictionary = new Dictionary<int, TcpListenerThread>();
		}

		internal static TcpListenerThread GetTcpListener(int port, IPAddress ip, ListenerTypeEnum listenerTypeEnum)
		{
			if(!_listenerDictionary.ContainsKey(port))
			{
				if(listenerTypeEnum == ListenerTypeEnum.Server)
				{
					_listenerDictionary[port] = new TcpServerListenerThread(port, ip);
				}
				else
				{
					_listenerDictionary[port] = new TcpClientListenerThread(port, ip);
				}
			}

			return _listenerDictionary[port];
		}
	}
}
