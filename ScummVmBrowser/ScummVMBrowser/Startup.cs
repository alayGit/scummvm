using ConfigStore;
using IceRpc;
using IceRpc.IceServerImplementations;
using ManagedCommon.Base;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ManagedCommon.Interfaces.Rpc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using PortSharer;
using ScummVMBrowser.Clients;
using ScummVMBrowser.Data;
using ScummVMBrowser.Models;
using ScummVMBrowser.Server;
using ScummVMBrowser.Utilities;
using SignalRHostWithUnity;
using SignalRHostWithUnity.Unity;
using StartInstance;
using System;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Lifetime;
using ManagedCommon.ExtensionMethods;
using ScummWebsServerVMSlices;
using System.Collections.Generic;
using IceRpc.I;
using ManagedCommon.Implementations;
using SignalR;
using ManagedCommon.Enums.Logging;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR.Json;


[assembly: OwinStartup(typeof(ScummVMBrowser.Startup))]

namespace ScummVMBrowser
{
    public class Startup
    {
        public static IUnityContainer Container { get; private set; }

        private IScummWebServerRpc _rpcServer { get; set; }

        public void Configuration(IAppBuilder app)
        {
            //PreStartCleanUp();
            // Branch the pipeline here for requests that start with "/signalr"

            IUnityContainer container = UnityConfiguration.SetConfiguredContainer(RegisterDependencies);

            try
            {
                DependencyResolver.SetResolver(new UnityDependencyResolver(container));
                GlobalHost.DependencyResolver.Register(typeof(IHubActivator),
                    () => new UnityHubActivator(container));
       
                _rpcServer = container.Resolve<IScummWebServerRpc>();


                JsonSerializerSettings serializerSettings = JsonUtility.CreateDefaultSerializerSettings();
                
                GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => JsonSerializer.Create(serializerSettings));

                GlobalHost.HubPipeline.AddModule(container.Resolve<ErrorHandlingPipelineModule>());

                app.Map("/signalr", map =>
                {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration
                    {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
                });

                app.OnDisposing(() =>
                {
                    _rpcServer.Dispose();
                });
            }
            catch(Exception e)
            {
                container.Resolve<ILogger>().LogMessage(LoggingLevel.Error, LoggingCategory.ScummVmWebBrowser, ErrorMessage.FailedToStartError, e.ToString());

                throw;
            }

        }

        private static void PreStartCleanUp()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "C:\\ScummVMNew\\ScummVMWeb\\GIT\\dists\\ScummVMBrowser\\KillSignalR.bat";

            Process.Start(processStartInfo);
        }

        static void RegisterDependencies(IUnityContainer container)
        {
            RpcClientManager<string, ScummWebServerClientPrx> clientManager = new RpcClientManager<string, ScummWebServerClientPrx>();

            container.RegisterInstance(container);
            container.RegisterInstance<IDictionary<string, ScummWebServerClientPrx>>(clientManager);
            container.RegisterInstance<IRpcClientProxyRemover<string>>(clientManager);
            container.RegisterInstance<IReadOnlyDictionary<string, ScummWebServerClientPrx>>(clientManager);
            container.RegisterInstance<IPortSender>(null); //Don't need one as the client is JavaScript so we should use a known port

            container.RegisterType<IConfigurationStore<System.Enum>, JsonConfigStore>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameInfo, GameInfo>();
            container.RegisterType<IHubActivator, UnityHubActivator>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGameClientStore<IGameInfo>, HubStore<GameInfo>>(new ContainerControlledLifetimeManager());
            container.RegisterType<IHubServer, HubServer>();
            container.RegisterType<IHubConnectionFactory, HubConnectionFactory>();
            container.RegisterType<IScummVMServerStarter, ScummVMServerStarter>();
            container.RegisterType<IScummVMHubClient, ScummVMHubClient>();
            container.RegisterType<IScummHubRpcAsyncProxy, ScummVmIceClient>();
            container.RegisterType<IPortGetter, SharedMemoryPortGetter>();
            container.RegisterType<IScummWebServerClientRpcProxy, ScummWebIceServer>();
            container.RegisterType<IScummWebServerRpc, RpcServer>();
            container.RegisterType<ILogger, WindowsEventLogger>();
            container.RegisterType<ErrorHandlingPipelineModule, ScummVmBrowserSignalRHandlingPipelineModule>();
			container.RegisterType<ICompression, ManagedZLibCompression.ManagedZLibCompression>();

            container.RegisterFactory<IStarter>(
                    s =>
                    {
                        return new SetPortStarter() { Port = container.Resolve<IConfigurationStore<System.Enum>>().GetValue<int>(IceRemoteProcFrontEnd.Port) }; //TODO: load port from config
                    }
                );

            container.RegisterFactory<IScummVMHubClient>(
                   c =>
                   {
                       SignalRRealTimDataBusAndEndpointClient signalRRealTimDataBusAndEndpointClient = new SignalRRealTimDataBusAndEndpointClient(container.Resolve<IConfigurationStore<System.Enum>>(), container.Resolve<IHubConnectionFactory>());
                       return new ScummVMHubClient(c.Resolve<IScummVMServerStarter>(), c.Resolve<IConfigurationStore<System.Enum>>(), signalRRealTimDataBusAndEndpointClient, c.Resolve<IScummHubRpcAsyncProxy>(), signalRRealTimDataBusAndEndpointClient, c.Resolve<IPortGetter>(), c.Resolve<IPortGetter>());
                   }
                );

            ScummWebServerI.Container = container;
        }
    }
}
