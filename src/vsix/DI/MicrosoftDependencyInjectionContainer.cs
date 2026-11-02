using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace TwinCAT.XAE.StartPage.DI
{
    using Core.Interfaces;

    internal class MicrosoftDependencyInjectionContainer : IDependencyInjectionContainer
    {
        private static readonly ServiceCollection _serviceCollection = new();
        private static ServiceProvider _serviceProvider = null;

        public void AddSingleton<TService>()
            => _serviceCollection.AddSingleton(typeof(TService));

        public void AddSingleton<TService>(TService implementation)
            => _serviceCollection.AddSingleton(typeof(TService), implementation);

        public void AddSingleton<TService, TImplementation>()
            => _serviceCollection.AddSingleton(typeof(TService), typeof(TImplementation));

        internal bool IsRegistered(Type serviceType)
            => _serviceCollection.Any(x => x.ServiceType == serviceType);

        public T GetInstance<T>()
            => (T)GetInstance(typeof(T));

        public object GetInstance(Type serviceType)
        {
            _serviceProvider ??= _serviceCollection.BuildServiceProvider();

            return _serviceProvider.GetService(serviceType);
        }
    }
}
