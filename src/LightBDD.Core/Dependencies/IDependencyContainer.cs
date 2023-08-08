using System;

namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Interface describing dependency container that is able to resolve dependencies as well as create inner scopes.
    /// </summary>
    //TODO: drop IDisposable
    public interface IDependencyContainer : IDependencyResolver, IAsyncDisposable
    {
        /// <summary>
        /// Creates new scope based on the current container.
        /// The exact behavior of the new scopes will depend on the implementation, but there is a general expectation that objects resolved with the new scope should be disposed with scope disposal.
        /// The created scope should be disposed after use and should be disposed before the parent scope.
        /// </summary>
        /// <returns>New scope.</returns>
        IDependencyContainer BeginScope();
    }
}
