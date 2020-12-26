using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Enums
{
   public enum SoundSettings
    {
        SampleSize,
        SampleRate,
        SoundPollingFrequencyMs,
        MaxQueuedToStopAudio,
        MinQueuedToResumeAudio,
        ClientFeedSize,
		ServerFeedSize
    }
}
