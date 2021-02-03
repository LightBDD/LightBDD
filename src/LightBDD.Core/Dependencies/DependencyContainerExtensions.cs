using System;
using LightBDD.Core.Dependencies.Implementation;

namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Extension class for <see cref="IDependencyContainer"/>.
    /// </summary>
    public static class DependencyContainerExtensions
    {
        /// <summary>
        /// Resolves dependency of the specified type.
        /// </summary>
        /// <returns>Resolved instance.</returns>
        public static IDependencyContainer BeginScope(this IDependencyContainer container, LifetimeScope scope, Action<ContainerConfigurator> configuration = null)
        {
            var v2 = (container as IDependencyContainerV2)
                     ?? new WrappingDependencyContainer(container);
            return v2.BeginScope(scope, configuration);
        }
    }
}