using IceRpc.I;
using ManagedCommon.Enums.Logging;
using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc.DispatchInterceptor
{
    internal class ScummWebLoggingServerInterceptor : LoggingInterceptor<ScummWebServerI>
    {
        public ScummWebLoggingServerInterceptor(ScummWebServerI servant, ILogger logger) : base(servant, logger)
        {
        }

        protected override void LogMessage(string error)
        {
            Logger.LogMessage(LoggingLevel.Error, LoggingCategory.ScummVmWebBrowser, ErrorMessage.IceCommuncationErrorScummVMBrowserFromClient, error);
        }
    }
}
