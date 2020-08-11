using ManagedCommon.Enums.Logging;
using ManagedCommon.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Implementations
{
    public class WindowsEventLogger : ILogger
    {
        public const string EventLogSource = "Application";
        public readonly static IReadOnlyDictionary<LoggingLevel, EventLogEntryType> LoggingLevelMap = new Dictionary<LoggingLevel, EventLogEntryType>() { { LoggingLevel.Error, EventLogEntryType.Error }, { LoggingLevel.Information, EventLogEntryType.Information }, { LoggingLevel.Warning, EventLogEntryType.Warning } };
        private readonly static IReadOnlyDictionary<ErrorMessage, string> _errorMessages;
        public const string ErrorMessagesResourceName = "ErrorMessages";

        static WindowsEventLogger()
        {
           _errorMessages = JsonConvert.DeserializeObject<Dictionary<ErrorMessage, string>>(Encoding.UTF8.GetString((byte[])ManagedCommon.Properties.Resources.ResourceManager.GetObject($"{ErrorMessagesResourceName}")));
        }

        public void LogMessage(LoggingLevel loggingLevel, LoggingCategory loggingCategory, ErrorMessage category, params string[] tokens)
        {
            using (EventLog eventLog = new EventLog(EventLogSource))
            {
                eventLog.Source = loggingCategory.ToString();
                eventLog.WriteEntry(string.Format(_errorMessages[category], tokens), LoggingLevelMap[loggingLevel], (int)category);
            }
        }
    }
}
