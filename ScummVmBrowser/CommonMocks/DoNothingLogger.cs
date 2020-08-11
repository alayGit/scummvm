using ManagedCommon.Enums.Logging;
using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMocks
{
    public class DoNothingLogger : ILogger
    {
        public void LogMessage(LoggingLevel loggingLevel, LoggingCategory loggingCategory, ErrorMessage category, params string[] tokens)
        {
            
        }
    }
}
