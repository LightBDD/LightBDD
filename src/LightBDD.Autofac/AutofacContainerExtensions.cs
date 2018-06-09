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
            var autofacScope = new AutofacContainer();
            autofacScope.AutofacScope = container.BeginLifetimeScope(builder => autofacScope.RegisterSelf(builder));

            configuration.UseContainer(autofacScope);
            return configuration;
        }

        public static DependencyContainerConfiguration UseAutofac(
            this DependencyContainerConfiguration configuration,
            ContainerBuilder builder)
        {
            var container = new AutofacContainer();
            container.AutofacScope = container.RegisterSelf(builder).Build();

            configuration.UseContainer(container);
            return configuration;
        }
    }
}