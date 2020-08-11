using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trasher;

namespace TestMocks
{
    public class MockAntiDisposalLock<T> : IAntiDisposalLock<T> where T : IDisposable
    {
        public MockAntiDisposalLock(T obj)
        {
            Obj = obj;
        }

        public T Obj { get; private set; }

        public void Dispose()
        {
        }
    }
}
