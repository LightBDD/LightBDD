using System;
using Autofac;
using LightBDD.Core.Dependencies;

namespace LightBDD.Autofac.Implementation
{
    internal class AutofacContainer : IDependencyContainer
    {
        private readonly ILifetimeScope _lifetimeScope;

        public AutofacContainer(ContainerBuilder builder)
            : this(builder.Build())
        {
        }

        public AutofacContainer(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public object Resolve(Type type)
        {
            return _lifetimeScope.Resolve(type);
        }

        public void Dispose()
        {
            _lifetimeScope.Dispose();
        }

        public IDependencyContainer BeginScope(Action<IContainerConfigurer> configuration = null)
        {
            return new AutofacContainer(_lifetimeScope.BeginLifetimeScope(builder => new AutofacContainerBuilder(builder).Configure(configuration)));
        }
    }
}