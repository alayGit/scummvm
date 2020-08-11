using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Exceptions
{
   public class UnmanagedException: Exception
    {
        public UnmanagedException(string message):base(message)
        {

        }
    }
}
