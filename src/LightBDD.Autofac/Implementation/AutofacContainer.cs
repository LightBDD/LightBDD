using System;
using Autofac;
using LightBDD.Core.Dependencies;

namespace LightBDD.Autofac.Implementation
{
    internal class AutofacContainer : IDependencyContainer
    {
        public ILifetimeScope AutofacScope { get; set; }
        public ILifetimeScope ParentScope { get; set; }

        public object Resolve(Type type)
        {
            return AutofacScope.Resolve(type);
        }

        public void Dispose()
        {
            AutofacScope.Dispose();
            ParentScope?.Dispose();
        }

        public IDependencyContainer BeginScope(Action<ContainerConfigurator> configuration = null)
        {
            var innerScope = new AutofacContainer();
            innerScope.AutofacScope = AutofacScope.BeginLifetimeScope(builder =>
            {
                new AutofacContainerBuilder(builder).Configure(configuration);
                innerScope.RegisterSelf(builder);
            });
            return innerScope;
        }

        public ContainerBuilder RegisterSelf(ContainerBuilder builder)
        {
            builder.RegisterInstance(this).As<IDependencyContainer>().As<IDependencyResolver>().ExternallyOwned();
            return builder;
        }
    }
}