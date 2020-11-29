using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
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
        Task EnqueueStringAsync(string toSend);
        Task EnqueueControlKeyAsync(ControlKeys toSend);
        Task EnqueueMouseMoveAsync(int x, int y);
        Task EnqueueMouseClickAsync(MouseClick mouseClick);
        Task StartSoundAsync();
        Task StopSoundAsync();
        Task<List<ScreenBuffer>> GetRedrawWholeScreenBuffersCompressed();
    }
}
