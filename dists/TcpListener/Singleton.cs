using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	internal static class Singleton
	{
		private static TcpListenerThread TcpListener;

		internal static TcpListenerThread GetTcpListener(int port, ListenerTypeEnum listenerTypeEnum)
		{
			if(TcpListener == null)
			{
				if(listenerTypeEnum == ListenerTypeEnum.Server)
				{
					TcpListener = new TcpServerListenerThread(port);
				}
				else
				{
					TcpListener = new TcpClientListenerThread(port);
				}
			}

			return TcpListener;
		}
	}
}
