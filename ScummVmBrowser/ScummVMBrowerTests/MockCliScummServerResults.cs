using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowserTests
{
   public class MockCliScummServerResults
    {
        public MockCliScummServerResults()
        {
            EnqueuedString = new List<string>();
        }

        public List<string> EnqueuedString { get; set; }
        public bool HasStarted { get; set; }

        public bool HasQuit { get; set; }
    }
}
