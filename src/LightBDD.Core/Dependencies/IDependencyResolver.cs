using System;

namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Interface allowing to resolve dependencies.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Resolves dependency of type specified by <paramref name="type"/> parameter.
        /// </summary>
        /// <param name="type">Dependency type.</param>
        /// <returns>Resolved dependency.</returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolves dependency of the specified type.
        /// </summary>
        /// <typeparam name="TDependency">Type of dependency.</typeparam>
        /// <returns>Resolved instance.</returns>
        public TDependency Resolve<TDependency>();
    }
}