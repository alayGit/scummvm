using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SignalRSelfHostTest
{
    public class MockClient : IHubCallerConnectionContext<object>
    {
        public MockNextFrame NextFrame { get; set; }

        public object Caller => throw new NotImplementedException();

        public dynamic CallerState => throw new NotImplementedException();

        public object Others => throw new NotImplementedException();

        public object All => throw new NotImplementedException();

        public object AllExcept(params string[] excludeConnectionIds)
        {
            throw new NotImplementedException();
        }

        public object Client(string connectionId)
        {
            return NextFrame;
        }

        public object Clients(IList<string> connectionIds)
        {
            throw new NotImplementedException();
        }

        public object Group(string groupName, params string[] excludeConnectionIds)
        {
            throw new NotImplementedException();
        }

        public object Groups(IList<string> groupNames, params string[] excludeConnectionIds)
        {
            throw new NotImplementedException();
        }

        public object OthersInGroup(string groupName)
        {
            throw new NotImplementedException();
        }

        public object OthersInGroups(IList<string> groupNames)
        {
            throw new NotImplementedException();
        }

        public object User(string userId)
        {
            throw new NotImplementedException();
        }

        public object Users(IList<string> userIds)
        {
            throw new NotImplementedException();
        }
    }
}
