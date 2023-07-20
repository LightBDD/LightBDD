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
        /// Configures LightBDD to use DI container described by <paramref name="serviceProvider"/> provider, where <paramref name="takeOwnership"/> specifies if LightBDD should control provider disposal or not.<br/>
        /// Please note that the new scope will be created to handle injections for LightBDD.<br/>
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="serviceProvider">Service provider to use.</param>
        /// <param name="takeOwnership">If true, the provider will be disposed by LightBDD after tests are finished.</param>
        public static DependencyContainerConfiguration UseContainer(
            this DependencyContainerConfiguration configuration, IServiceProvider serviceProvider, bool takeOwnership)
        {
            return UseContainer(configuration, serviceProvider, opt => opt.TakeOwnership(takeOwnership));
        }

        /// <summary>
        /// Configures LightBDD to use DI container described by <paramref name="serviceProvider"/> provider, and allows customizing container behaviour with <paramref name="options"/> delegate.<br/>
        /// </summary>
        public static DependencyContainerConfiguration UseContainer(
            this DependencyContainerConfiguration configuration, IServiceProvider serviceProvider, Action<DiContainerOptions> options)
        {
            var containerOptions = new DiContainerOptions();
            options?.Invoke(containerOptions);

            var scope = new NestingContainerScope(serviceProvider.CreateScope(), containerOptions.ShouldEnableScopeNestingWithinScenarios);
            var container = new DiContainer(scope, new ContainerOverrides());

            if (containerOptions.ShouldTakeOwnership)
            {
                if (serviceProvider is IDisposable disposableParent)
                    container.AddDisposable(disposableParent);
                else
                    throw new ArgumentException($"The provided {serviceProvider.GetType().FullName} is not {nameof(IDisposable)} and LightBDD cannot take proper ownership of the provider. Please consider disabling takeOwnership configuration flag and manual disposal of the provider.", nameof(serviceProvider));
            }

            return configuration.UseContainer(container);
        }
    }
}