using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Enums.Logging
{
    public  enum ErrorMessage
    {
        GeneralErrorScummVMBrowser,
        GeneralErrorCliScumm,
        IceCommuncationErrorScummVMBrowserFromClient,
        IceCommuncationErrorScummVMBrowserFromCliScumm,
        IceCommuncationErrorCliScumm,
        SignalRScummVmBrowserError,
        SignalRCliScummError,
        UnmanagedError,
        WrapperError,
        FailedToStartError,
        YEncodeFailure,
        YDecodeFailure,
    }
}
