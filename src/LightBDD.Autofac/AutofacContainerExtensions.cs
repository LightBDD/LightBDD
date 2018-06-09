using Autofac;
using LightBDD.Autofac.Implementation;
using LightBDD.Core.Configuration;

namespace LightBDD.Autofac
{
    public static class AutofacContainerExtensions
    {
        public static DependencyContainerConfiguration UseAutofac(
            this DependencyContainerConfiguration configuration,
            ILifetimeScope container)
        {
            configuration.UseContainer(new AutofacContainer(container.BeginLifetimeScope()));
            return configuration;
        }

        public static DependencyContainerConfiguration UseAutofac(
            this DependencyContainerConfiguration configuration,
            ContainerBuilder builder)
        {
            configuration.UseContainer(new AutofacContainer(builder));
            return configuration;
        }
    }
}