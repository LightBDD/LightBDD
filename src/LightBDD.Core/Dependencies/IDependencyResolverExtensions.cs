namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Extension methods for <see cref="IDependencyResolver"/>.
    /// </summary>
    public static class IDependencyResolverExtensions
    {
        /// <summary>
        /// Resolves dependency of type specified by <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Dependency type.</typeparam>
        /// <param name="resolver">Dependency resolver.</param>
        /// <returns>Resolved dependency.</returns>
        public static T Resolve<T>(this IDependencyResolver resolver)
        {
            return (T)resolver.Resolve(typeof(T));
        }
    }
}