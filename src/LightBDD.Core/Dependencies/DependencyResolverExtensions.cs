namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Extension class for <see cref="IDependencyResolver"/>.
    /// </summary>
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// Resolves dependency of the specified type.
        /// </summary>
        /// <typeparam name="TDependency">Type of dependency.</typeparam>
        /// <param name="resolver">Resolver.</param>
        /// <returns>Resolved instance.</returns>
        public static TDependency Resolve<TDependency>(this IDependencyResolver resolver)
            => (TDependency)resolver.Resolve(typeof(TDependency));
    }
}