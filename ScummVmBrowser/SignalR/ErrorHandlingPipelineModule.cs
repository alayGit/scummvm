using ManagedCommon.Enums.Logging;
using ManagedCommon.Interfaces;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR
{
   public abstract class ErrorHandlingPipelineModule: HubPipelineModule
    {
		protected ILogger Logger { get; private set; }
		
		public ErrorHandlingPipelineModule(ILogger logger)
		{
			Logger = logger;
		}
		protected override void OnIncomingError(ExceptionContext exceptionContext,
			 IHubIncomingInvokerContext invokerContext)
		{
			StringBuilder sBuilder = new StringBuilder(string.Empty);
			sBuilder.Append(exceptionContext.Error);

			//Exception inner = exceptionContext.Error.InnerException;
			for(Exception exception = exceptionContext.Error.InnerException; exception != null; exception = exception.InnerException)
			{
				sBuilder.Append($"\n{exceptionContext.Error.InnerException}");
			}

			LogMessage(sBuilder.ToString(), invokerContext.Hub.Context.ConnectionId);

			base.OnIncomingError(exceptionContext, invokerContext);
		}

		protected abstract void LogMessage(string error, string connectionId);
	}
}
