using Ice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceRpc.Interfaces
{
   public interface ICallBackRegisterer<T> where T: ObjectPrx
    {
        void addClient(T receiver, Current current);
    }
}
