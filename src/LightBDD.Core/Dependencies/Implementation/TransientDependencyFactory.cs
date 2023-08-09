#nullable enable
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace LightBDD.Core.Dependencies.Implementation
{
    internal static class TransientDependencyFactory
    {
        private static readonly ConcurrentDictionary<Type, Func<IDependencyResolver, object>> CtorCache = new();

        public static object Create(Type type, IDependencyResolver resolver)
        {
            return CtorCache.GetOrAdd(type, CreateConstructorInitializer).Invoke(resolver);
        }

        private static Func<IDependencyResolver, object> CreateConstructorInitializer(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsAbstract || typeInfo is { IsClass: false, IsValueType: false })
                return _ => throw new InvalidOperationException($"Type '{type}' has to be non-abstract class or value type.");

            var ctors = typeInfo.DeclaredConstructors.Where(x => x.IsPublic).ToArray();

            if (ctors.Length != 1)
                return _ => throw new InvalidOperationException($"Type '{type}' has to have exactly one public constructor (number of public constructors: {ctors.Length}).");

            var ctor = ctors[0];
            return r => ctor.Invoke(ctor.GetParameters().Select(p => r.Resolve(p.ParameterType)).ToArray());
        }
    }
}