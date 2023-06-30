using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace LightBDD.Core.Dependencies.Implementation
{
    class DependencyDescriptor
    {
        private static ConcurrentDictionary<Type, Func<IDependencyResolver, object>> _ctorCache = new();
        public Type Type { get; }
        public Func<IDependencyResolver, object> ResolveFn { get; }
        public RegistrationOptions Registration { get; }
        public InstanceScope Scope { get; }
        public bool InstantResolution { get; }

        public DependencyDescriptor(Type type, Func<IDependencyResolver, object> resolveFn, RegistrationOptions registration, InstanceScope scope, bool instantResolution)
        {
            Type = type;
            ResolveFn = resolveFn;
            Registration = registration;
            if (!registration.AsTypes.Any())
                registration.As(type);
            Scope = scope;
            InstantResolution = instantResolution;
        }

        public override string ToString() => $"{Type} ({Scope})";

        public static Func<IDependencyResolver, object> FindConstructor(Type type)
        {
            return _ctorCache.GetOrAdd(type, CreateConstructorInitializer);
        }

        private static Func<IDependencyResolver, object> CreateConstructorInitializer(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsAbstract || (!typeInfo.IsClass && !typeInfo.IsValueType))
                return _ => throw new InvalidOperationException($"Type '{type}' has to be non-abstract class or value type.");

            var ctors = typeInfo.DeclaredConstructors.Where(x => x.IsPublic).ToArray();

            if (ctors.Length != 1)
                return _ => throw new InvalidOperationException($"Type '{type}' has to have have exactly one public constructor (number of public constructors: {ctors.Length}).");

            var ctor = ctors[0];
            return r => ctor.Invoke(ctor.GetParameters().Select(p => r.Resolve(p.ParameterType)).ToArray());
        }
    }
}