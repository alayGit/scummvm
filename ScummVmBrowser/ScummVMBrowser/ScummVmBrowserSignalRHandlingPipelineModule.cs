using ManagedCommon.Enums.Logging;
using ManagedCommon.Interfaces;
using SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScummVMBrowser
{
    public class ScummVmBrowserSignalRHandlingPipelineModule : ErrorHandlingPipelineModule
    {
        private IGameClientStore<IGameInfo> _hubStore;

        public ScummVmBrowserSignalRHandlingPipelineModule(IGameClientStore<IGameInfo> hubStore, ILogger logger) : base(logger)
        {
            _hubStore = hubStore;
        }

        protected override void LogMessage(string error, string connectionId)
        {
            Logger.LogMessage(LoggingLevel.Error, LoggingCategory.ScummVmWebBrowser, ErrorMessage.SignalRScummVmBrowserError, _hubStore.GetGameIdByConnectionId(connectionId), error);
        }
    }
}