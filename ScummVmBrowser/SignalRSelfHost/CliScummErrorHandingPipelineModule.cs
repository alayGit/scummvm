using ManagedCommon.Enums.Logging;
using ManagedCommon.Interfaces;
using SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRSelfHost
{
    public class CliScummErrorHandingPipelineModule : ErrorHandlingPipelineModule
    {
        public CliScummErrorHandingPipelineModule(ILogger logger) : base(logger)
        {
        }

        protected override void LogMessage(string error, string connectionId)
        {
            Logger.LogMessage(LoggingLevel.Error, LoggingCategory.CliScummSelfHost, ErrorMessage.SignalRCliScummError, error);
        }
    }
}
