﻿using ManagedCommon.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartInstance
{
   public interface IStarter
    {
        bool StartConnection(StartConnection startConnection);
    }
}
