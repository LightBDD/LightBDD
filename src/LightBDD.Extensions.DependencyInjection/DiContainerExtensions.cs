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
        /// Please note that the new scope will be created to handle injections for LightBDD.<br/>
        /// Please note that <paramref name="serviceProvider"/> will not be disposed by LightBDD after test run, as it is treated as externally owned instance
        /// - use <see cref="UseContainer(DependencyContainerConfiguration,IServiceProvider,bool)"/> if container should be fully managed by LightBDD.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="serviceProvider">Service provider to use.</param>
        [Obsolete("Use overload with takeOwnership flag instead", true)]
        public static DependencyContainerConfiguration UseContainer(
            this DependencyContainerConfiguration configuration, IServiceProvider serviceProvider)
        {
            return UseContainer(configuration, serviceProvider, false);
        }

        /// <summary>
        /// Configures LightBDD to use provided <paramref name="serviceProvider"/> provider, where <paramref name="takeOwnership"/> specifies if LightBDD should control provider disposal or not.<br/>
        /// Please note that the new scope will be created to handle injections for LightBDD.<br/>
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="serviceProvider">Service provider to use.</param>
        /// <param name="takeOwnership">If true, the provider will be disposed by LightBDD after tests are finished.</param>
        public static DependencyContainerConfiguration UseContainer(
            this DependencyContainerConfiguration configuration, IServiceProvider serviceProvider, bool takeOwnership)
        {
            var container = new DiContainer(serviceProvider.CreateScope(), new ContainerOverrides());

            if (takeOwnership)
            {
                if (serviceProvider is IDisposable disposableParent)
                    container.AddDisposable(disposableParent);
                else
                    throw new ArgumentException($"The provided {serviceProvider.GetType().FullName} is not {nameof(IDisposable)} and LightBDD cannot take proper ownership of the provider. Please consider specifying {nameof(takeOwnership)} parameter to be false and manual disposal of the provider.", nameof(serviceProvider));
            }

            return configuration.UseContainer(container);
        }
    }
}