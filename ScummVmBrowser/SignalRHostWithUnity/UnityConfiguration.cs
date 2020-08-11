using System;
using Unity;
using Unity.Lifetime;

namespace SignalRHostWithUnity.Unity
{
    public class UnityConfiguration
    {
        public delegate void RegisterTypes(IUnityContainer container);
        #region Unity Container
        private static Lazy<IUnityContainer> Container(RegisterTypes registerTypes)
        {
            return new Lazy<IUnityContainer>(() =>
            {
                var container = new UnityContainer();
                registerTypes(container);
                return container;
            });
        }

        public static IUnityContainer SetConfiguredContainer(RegisterTypes registerTypes)
        {
            return Container(registerTypes).Value;
        }
        #endregion
    }
}