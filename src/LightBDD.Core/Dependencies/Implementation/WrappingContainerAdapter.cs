using System;

namespace LightBDD.Core.Dependencies.Implementation
{
    internal class WrappingContainerAdapter : IDependencyContainerV2
    {
        private readonly IDependencyContainer _v1;

        public WrappingContainerAdapter(IDependencyContainer v1)
        {
            _v1 = v1;
        }

        public object Resolve(Type type)
        {
            return _v1.Resolve(type);
        }

        public void Dispose()
        {
            _v1.Dispose();
        }

        public IDependencyContainer BeginScope(Action<ContainerConfigurator> configuration = null) => BeginScope(LifetimeScope.Local, configuration);

        public IDependencyContainerV2 BeginScope(LifetimeScope scope, Action<ContainerConfigurator> configuration = null)
        {
            return new WrappingContainerAdapter(_v1.BeginScope(configuration));
        }
    }
}