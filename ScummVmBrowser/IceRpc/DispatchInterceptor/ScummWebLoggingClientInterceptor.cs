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
    internal class ScummWebLoggingClientInterceptor : LoggingInterceptor<ScummHubClientI>
    {
        public ScummWebLoggingClientInterceptor(ScummHubClientI servant, ILogger logger) : base(servant, logger)
        {
        }

        protected override void LogMessage(string error)
        {
            Logger.LogMessage(LoggingLevel.Error, LoggingCategory.ScummVmWebBrowser, ErrorMessage.IceCommuncationErrorScummVMBrowserFromCliScumm, error);
        }
    }
}
