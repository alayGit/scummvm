using CLIScumm;
using ConfigStore;
using IceRpc;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Logging;
using ManagedCommon.Implementations;
using ManagedCommon.Interfaces;
using ManagedCommon.Interfaces.Rpc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using PortSharer;
using ScummVMBrowser.Utilities;
using SignalR;
using SignalRHostWithUnity.Unity;
using StartInstance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace SignalRSelfHost
{
    class WebServer
    {
        internal static bool GameHasEnded;

        static async Task Main(string[] args)
        {
            UnityContainer container = new UnityContainer();
            RegisterDependencies(container);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.FirstChanceException += new EventHandler<FirstChanceExceptionEventArgs>((o,e) => container.Resolve<ILogger>().LogMessage(LoggingLevel.Error, LoggingCategory.CliScummSelfHost, ErrorMessage.GeneralErrorCliScumm, e.Exception.ToString()));
            try
            {
                if (args.Length != 2)
                {
                    throw new Exception($"Incorrect args passed into CliScummSelf Host. Need 2 portGetterIds. The args were: {string.Join(" ", args)}");
                }

                GlobalHost.DependencyResolver.Register(typeof(IHubActivator),
               () => new UnityHubActivator(container));
                GlobalHost.HubPipeline.AddModule(container.Resolve<ErrorHandlingPipelineModule>());

                IConfigurationStore<System.Enum> configurationStore = container.Resolve<IConfigurationStore<System.Enum>>();

				CliScumm scummHub = container.Resolve<CliScumm>(); //TODO: Add interface
                await scummHub.Init(args[0], args[1]);
            }
            catch(Exception e)
            {
                container.Resolve<ILogger>().LogMessage(LoggingLevel.Error, LoggingCategory.CliScummSelfHost, ErrorMessage.FailedToStartError, e.ToString());
                throw;
            }

            while (!GameHasEnded)
            {
               await Task.Delay(2000);
            }

        }

        static void RegisterDependencies(IUnityContainer container)
        {
            container.RegisterType<ScummVMSignalRHub, ScummVMSignalRHub>(new ContainerControlledLifetimeManager());
            container.RegisterType<IWrapper, Wrapper>(
                 new ContainerControlledLifetimeManager()
           );
            container.RegisterType<IHubActivator, UnityHubActivator>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationStore<System.Enum>, JsonConfigStore>(new ContainerControlledLifetimeManager());
            container.RegisterType<IRealTimeDataBusServer, ScummVMSignalRHub>(new ContainerControlledLifetimeManager());
            container.RegisterType<IScummHubClientRpcProxy, ScummVmIceServer>(new ContainerControlledLifetimeManager());
            container.RegisterType<IRealTimeEndPointCallbackRepo,SignalRMemoryRealTimeEndpointServer>(new ContainerControlledLifetimeManager());
            container.RegisterType<CliScumm, CliScumm>(); //ToDo Make Interface
            container.RegisterType<IPortSender, SharedMemoryPortSender>();
            container.RegisterType<IStarter, DynamicInstanceStarter>();
            container.RegisterType<ILogger, WindowsEventLogger>();
            container.RegisterType<ErrorHandlingPipelineModule, CliScummErrorHandingPipelineModule>();
			container.RegisterType<IByteEncoder, ManagedYEncoder.ManagedYEncoder>();

            container.RegisterInstance(container.Resolve<IRealTimeEndPointCallbackRepo>() as IRealTimeDataEndpointServer); 
        }
    }
}

