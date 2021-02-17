using System;
using LightBDD.Core.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Extensions.DependencyInjection.Implementation
{
    class NestingContainerScope : IContainerScope
    {
        private readonly IServiceScope _serviceScope;
        private readonly bool _allowNestedScopesInScenario;

        public NestingContainerScope(IServiceScope serviceScope, bool allowNestedScopesInScenario)
        {
            _serviceScope = serviceScope;
            _allowNestedScopesInScenario = allowNestedScopesInScenario;
        }

        public void Dispose() => _serviceScope.Dispose();

        public object Resolve(Type type) => _serviceScope.ServiceProvider.GetRequiredService(type);

        public IContainerScope BeginScope(LifetimeScope scope)
        {
            if (!_allowNestedScopesInScenario && scope.Equals(LifetimeScope.Scenario))
                return new FlatContainerScope(_serviceScope.ServiceProvider.CreateScope(), false);
            
            return new NestingContainerScope(_serviceScope.ServiceProvider.CreateScope(), _allowNestedScopesInScenario);
        }
    }
}