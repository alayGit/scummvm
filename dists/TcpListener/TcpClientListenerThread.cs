using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	class TcpClientListenerThread : TcpListenerThread
	{
		int _port;
		TcpClient _client;

		NetworkStream _clientStream;

		public TcpClientListenerThread(int port)
		{
			_port = port;
		}

		protected async override Task<NetworkStream> ClientStream()
		{
			if (_client == null)
			{
				_client = new TcpClient();
				await _client.ConnectAsync(IPAddress.Parse("127.0.0.1"), _port);
				_clientStream = _client.GetStream();
			}

			return _clientStream;
		}
	}
}
