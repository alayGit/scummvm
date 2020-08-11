using ManagedCommon.Enums.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
   public interface ILogger
    {
        void LogMessage(LoggingLevel loggingLevel, LoggingCategory loggingCategory, ErrorMessage category, params string[] tokens);
    }
}
