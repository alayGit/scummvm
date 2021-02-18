using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using ManagedCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
   public interface IRealTimeDataBusClient
    {
        Task Init(string id);
        Task StartConnectionAsync();
		Task EnqueueInputMessages(KeyValuePair<string, string>[] inputMessages);
		Task StartSoundAsync();
        Task StopSoundAsync();
        Task<List<KeyValuePair<MessageType, string>>> ScheduleRedrawWholeScreen();
    }
}
