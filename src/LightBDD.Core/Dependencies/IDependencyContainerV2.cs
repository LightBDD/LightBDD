using System;

namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Interface describing dependency container that is able to resolve dependencies as well as create inner scopes.
    /// </summary>
    public interface IDependencyContainerV2 : IDependencyContainer
    {
        /// <summary>
        /// Creates new scope based on the current container and the additional <paramref name="configuration"/> if specified.
        /// The exact behavior of the new scopes will depend on the implementation, but there is a general expectation that objects resolved with the new scope should be disposed with scope disposal.
        /// The created scope should be disposed after use and should be disposed before the parent scope.
        /// </summary>
        /// <param name="scope">Container scope</param>
        /// <param name="configuration">An additional configuration that, if specified, should be applied on the scope.</param>
        /// <returns>New scope.</returns>
        IDependencyContainerV2 BeginScope(LifetimeScope scope, Action<ContainerConfigurator>? configuration = null);
    }
}