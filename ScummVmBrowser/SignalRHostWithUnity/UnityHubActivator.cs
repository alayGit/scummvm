using Microsoft.AspNet.SignalR.Hubs;
using System;
using Unity;

namespace SignalRHostWithUnity.Unity
{
    //The code for this assembly came from 'https://damienbod.com/2013/11/05/using-signalr-with-unity/' with some minor modifications
    public class UnityHubActivator : IHubActivator
    {
        private readonly IUnityContainer _container;

        public UnityHubActivator(IUnityContainer container)
        {
            _container = container;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            if (descriptor.HubType == null)
            {
                return null;
            }

            object hub = _container.Resolve(descriptor.HubType) ?? Activator.CreateInstance(descriptor.HubType);
            return hub as IHub;
        }
    }
}