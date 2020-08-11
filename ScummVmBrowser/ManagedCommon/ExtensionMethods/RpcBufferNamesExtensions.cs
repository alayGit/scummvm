using ManagedCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.ExtensionMethods
{
    public static class RpcBufferNamesExtensions
    {
        public static string AsBufferName(this RpcBufferNames rpcBufferName, string id)
        {
            return rpcBufferName.ToString() + id;
        }
    }
}