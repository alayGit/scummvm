using ManagedCommon.Enums;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScummVMBrowerTests
{
    public class MockHubProxy : IHubProxy
    { 
        public Subscription Subscription { get; set; }

        public JToken this[string name] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public JsonSerializer JsonSerializer => new JsonSerializer();

        public List<string> EnqueuedStrings { get; set; }
        public List<ControlKeys> EnqueuedControlKeys { get; set; }

        public MockHubProxy()
        {
            EnqueuedStrings = new List<string>();
            EnqueuedControlKeys = new List<ControlKeys>();
        }

        public Task Invoke(string method, params object[] args)
        {
            switch(method)
            {
                case "EnqueueString":
                    EnqueuedStrings.Add((string) args[0]);
                    break;
                case "EnqueueControlKey":
                    EnqueuedControlKeys.Add((ControlKeys)args[0]);
                    break;
            }
            return Task.CompletedTask;
        }

        public Task<T> Invoke<T>(string method, params object[] args)
        {
           return Task<T>.FromResult(default(T));
        }

        public Task Invoke<T>(string method, Action<T> onProgress, params object[] args)
        {
            return Task<T>.FromResult(default(T));
        }

        public Task<TResult> Invoke<TResult, TProgress>(string method, Action<TProgress> onProgress, params object[] args)
        {
            return Task<TResult>.FromResult(default(TResult));
        }

        public Subscription Subscribe(string eventName)
        {
             Subscription = new Subscription();

            return Subscription;
        }
    }
}
