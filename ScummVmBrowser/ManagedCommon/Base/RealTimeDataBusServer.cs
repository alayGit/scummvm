using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Base
{
	public abstract class RealTimeDataBusServer
	{
		private IPortSender _portSender;
		private IConfigurationStore<System.Enum> _configurationStore;
		private IStarter _starter;

		protected RealTimeDataBusServer(IPortSender portSender, IConfigurationStore<System.Enum> configurationStore, IStarter starter)
		{
			_portSender = portSender;
			_configurationStore = configurationStore;
			_starter = starter;
		}

		public virtual Task Init(string realTimePortGetterId)
		{
			int port = 0;
			string hostName = _configurationStore.GetValue(ScummConnectionSettings.CliScummHubHostName);

			bool result = _starter.StartConnection(
			   p =>
			   {
				   return StartConnection(p);
			   }
			 );

			if (!result)
			{
				throw new Exception($"Failed to start webServer on port {port}.");
			}

			_portSender.Init(() => port, realTimePortGetterId);

			string url = $"http://{hostName}:{port}";

			Console.WriteLine($"Server running on {port}", url);

			return Task.CompletedTask;
		}

		protected abstract bool StartConnection(int port);
	}
}
