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
    internal class ScummVmLoggingServerInterceptor : LoggingInterceptor<ScummHubI>
    {
        public ScummVmLoggingServerInterceptor(ScummHubI servant, ILogger logger) : base(servant, logger)
        {
        }

        protected override void LogMessage(string error)
        {
            Logger.LogMessage(LoggingLevel.Error, LoggingCategory.CliScummSelfHost, ErrorMessage.IceCommuncationErrorCliScumm, error);
        }
    }
}
