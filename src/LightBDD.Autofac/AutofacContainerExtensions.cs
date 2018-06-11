using Autofac;
using LightBDD.Autofac.Implementation;
using LightBDD.Core.Configuration;

namespace LightBDD.Autofac
{
    /// <summary>
    /// Extension class providing integration methods for Autofac DI container.
    /// </summary>
    public static class AutofacContainerExtensions
    {
        /// <summary>
        /// Configures LightBDD to use provided <paramref name="container"/> Autofac container.<br/>
        /// Please note that <paramref name="container"/> will not be disposed by LightBDD after test run, but rather treat it as externally owned instance
        /// - use <see cref="UseAutofac(DependencyContainerConfiguration,ContainerBuilder)"/> if container should be fully managed by LightBDD.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="container">Container to use.</param>
        public static DependencyContainerConfiguration UseAutofac(
            this DependencyContainerConfiguration configuration,
            ILifetimeScope container)
        {
            var autofacScope = new AutofacContainer();
            autofacScope.AutofacScope = container.BeginLifetimeScope(builder => autofacScope.RegisterSelf(builder));

            configuration.UseContainer(autofacScope);
            return configuration;
        }

        /// <summary>
        /// Configures LightBDD to use Autofac container described by <paramref name="builder"/>.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="builder">Autofac container builder.</param>
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