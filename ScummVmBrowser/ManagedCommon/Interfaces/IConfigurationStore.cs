using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
    public interface IConfigurationStore<in T>
    {
        V GetValue<V>(T key) where V: IConvertible;
        string GetValue(T key);
    }
}
