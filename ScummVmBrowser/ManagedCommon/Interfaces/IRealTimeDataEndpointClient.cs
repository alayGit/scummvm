using ManagedCommon.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Base
{
   public interface IRealTimeDataEndpointClient: IDisposable
    {
        void OnFrameReceived(CopyRectToScreenAsync copyRectToScreen, int instanceId);
        Task Init(string id);
        Task StartConnectionAsync();
    }
}
