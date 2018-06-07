using System;
using System.Threading.Tasks;

namespace LightBDD.Core.Execution.Dependencies
{
    public interface IDependencyResolver
    {
        Task<object> ResolveAsync(Type type);
    }

    public static class DependencyResolverExtensions
    {
        public static async Task<T> ResolveAsync<T>(this IDependencyResolver resolver)
            => (T)await resolver.ResolveAsync(typeof(T));
    }
}