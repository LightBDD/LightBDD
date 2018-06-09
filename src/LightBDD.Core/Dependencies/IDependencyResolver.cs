using System;
using System.Threading.Tasks;

namespace LightBDD.Core.Dependencies
{
    public interface IDependencyResolver
    {
        object Resolve(Type type);
    }

    public static class DependencyResolverExtensions
    {
        public static T Resolve<T>(this IDependencyResolver resolver)
            => (T)resolver.Resolve(typeof(T));
    }
}