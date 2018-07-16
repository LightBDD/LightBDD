using System;
using LightBDD.Core.Configuration;
using LightBDD.Extensions.DependencyInjection.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension class providing integration methods for Microsoft DI compatible containers.
    /// </summary>
    public static class DiContainerExtensions
    {
        /// <summary>
        /// Configures LightBDD to use DI container described by <paramref name="serviceProvider"/>.
        /// Please note that the new scope will be created to handle injections for LightBDD.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="serviceProvider">Service provider instance.</param>
        public static DependencyContainerConfiguration UseContainer(
            this DependencyContainerConfiguration configuration, IServiceProvider serviceProvider)
        {
            return configuration.UseContainer(new DiContainer(serviceProvider.CreateScope(), new ContainerOverrides()));
        }
    }
}