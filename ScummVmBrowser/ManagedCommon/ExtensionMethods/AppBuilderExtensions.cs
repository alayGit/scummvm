using Microsoft.Owin.BuilderProperties;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManagedCommon.ExtensionMethods
{
    public static class AppBuilderExtensions
    {
        public static void OnDisposing(this IAppBuilder app, Action cleanup)
        {
            var properties = new AppProperties(app.Properties);
            var token = properties.OnAppDisposing;
            if (token != CancellationToken.None)
            {
                token.Register(cleanup);
            }
        }
    }
}
