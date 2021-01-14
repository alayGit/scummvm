using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpRealTimeData
{
	class TcpServerListenerThread : TcpListenerThread
	{
		TcpListener _tcpListener;
		TcpClient _client;
		NetworkStream _networkStream;
		int _port;
		AsyncSemaphore _waitForConnectionSemaphore;

		public TcpServerListenerThread(int port)
		{
			_port = port;
			_waitForConnectionSemaphore = new AsyncSemaphore(1);
		}

		protected async override Task<NetworkStream> ClientStream()
		{
			if (_tcpListener == null || _client == null)
			{
				using (await _waitForConnectionSemaphore.EnterAsync())
				{
					if (_tcpListener == null || _client == null)
					{
						_tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port); //Converted from https://www.smartconversion.com/unit_conversion/IP_Address_Converter.aspx. It is actually 127.0.0.1
						_tcpListener.Start();

						_client = await _tcpListener.AcceptTcpClientAsync();
						_networkStream = _client.GetStream();
					}
				}
			}

			return _networkStream;
		}
	}
}
