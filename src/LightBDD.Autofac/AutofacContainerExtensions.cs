using System;
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
        /// Please note that the new scope will be created to handle injections for LightBDD.<br/>
        /// Please note that <paramref name="container"/> will not be disposed by LightBDD after test run, as it is treated as externally owned instance
        /// - use <see cref="UseAutofac(DependencyContainerConfiguration,ContainerBuilder)"/> or <see cref="UseAutofac(DependencyContainerConfiguration,ILifetimeScope,bool)"/> if container should be fully managed by LightBDD.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="container">Container to use.</param>

        [Obsolete("Use other methods instead", true)]
        public static DependencyContainerConfiguration UseAutofac(
            this DependencyContainerConfiguration configuration,
            ILifetimeScope container)
        {
            return configuration.UseAutofac(container, false);
        }

        /// <summary>
        /// Configures LightBDD to use provided <paramref name="container"/> Autofac container, where <paramref name="takeOwnership"/> specifies if LightBDD should control container disposal or not.<br/>
        /// Please note that the new scope will be created to handle injections for LightBDD.<br/>
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="container">Container to use.</param>
        /// <param name="takeOwnership">If true, the container will be disposed by LightBDD after tests are finished.</param>
        public static DependencyContainerConfiguration UseAutofac(
            this DependencyContainerConfiguration configuration,
            ILifetimeScope container,
            bool takeOwnership)
        {
            var autofacContainer = new AutofacContainer();
            autofacContainer.AutofacScope = container.BeginLifetimeScope(builder => autofacContainer.RegisterSelf(builder));
            if (takeOwnership)
                autofacContainer.ParentScope = container;

            return configuration.UseContainer(autofacContainer);
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

            return configuration.UseContainer(container);
        }
    }
}