using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowerTests
{
    class MockRequest : IRequest
    {
        public Uri Url => throw new NotImplementedException();

        public string LocalPath => throw new NotImplementedException();

        public INameValueCollection QueryString => throw new NotImplementedException();

        public INameValueCollection Headers => throw new NotImplementedException();

        public IDictionary<string, Cookie> Cookies => throw new NotImplementedException();

        public IPrincipal User => throw new NotImplementedException();

        public IDictionary<string, object> Environment => throw new NotImplementedException();

        public Task<INameValueCollection> ReadForm()
        {
            throw new NotImplementedException();
        }
    }
}
