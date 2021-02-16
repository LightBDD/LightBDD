using System;
using LightBDD.Core.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Extensions.DependencyInjection.Implementation
{
    internal class FlatContainerScope : IContainerScope
    {
        private readonly IServiceScope _serviceScope;
        private readonly bool _externallyOwned;

        public FlatContainerScope(IServiceScope serviceScope, bool externallyOwned)
        {
            _serviceScope = serviceScope;
            _externallyOwned = externallyOwned;
        }

        public void Dispose()
        {
            if (!_externallyOwned)
                _serviceScope.Dispose();
        }

        public object Resolve(Type type) => _serviceScope.ServiceProvider.GetRequiredService(type);

        public IContainerScope BeginScope(LifetimeScope scope) => new FlatContainerScope(_serviceScope, true);
    }
}