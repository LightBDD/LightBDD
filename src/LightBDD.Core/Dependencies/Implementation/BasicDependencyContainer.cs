using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace LightBDD.Core.Dependencies.Implementation
{
    internal class BasicDependencyContainer : ContainerConfigurator, IDependencyContainer
    {
        private readonly BasicDependencyContainer? _parent;
        private readonly ConcurrentQueue<IDisposable> _disposable = new ConcurrentQueue<IDisposable>();
        private readonly ConcurrentDictionary<Type, object> _items = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, Func<object>> _ctorCache = new ConcurrentDictionary<Type, Func<object>>();

        public BasicDependencyContainer(Action<ContainerConfigurator>? configuration = null) : this(null, configuration) { }
        private BasicDependencyContainer(BasicDependencyContainer? parent, Action<ContainerConfigurator>? configuration = null)
        {
            _parent = parent;
            configuration?.Invoke(this);
        }

        public object Resolve(Type type)
        {
            if (type == typeof(IDependencyContainer) || type == typeof(IDependencyResolver))
                return this;

            if (ResolveCached(this, type, out var cached))
                return cached;

            return EnlistDisposable(Create(type));
        }

        private static bool ResolveCached(BasicDependencyContainer container, Type type, out object cached)
        {
            var current = container;
            do
            {
                if (current._items.TryGetValue(type, out cached))
                    return true;

                current = current._parent;
            } while (current != null);

            return false;
        }

        private object Create(Type type)
        {
            try
            {
                return _ctorCache.GetOrAdd(type, FindConstructor).Invoke();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Unable to resolve type {type}:{Environment.NewLine}{e.Message}", e);
            }
        }

        private Func<object> FindConstructor(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsAbstract || (!typeInfo.IsClass && !typeInfo.IsValueType))
                return () => throw new InvalidOperationException($"Type '{type}' has to be non-abstract class or value type.");

            var ctors = typeInfo.DeclaredConstructors.Where(x => x.IsPublic).ToArray();

            if (ctors.Length != 1)
                return () => throw new InvalidOperationException($"Type '{type}' has to have have exactly one public constructor (number of public constructors: {ctors.Length}).");

            var ctor = ctors[0];
            return () => ctor.Invoke(ctor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray());
        }

        public void Dispose()
        {
            var exceptions = new List<Exception>();
            while (_disposable.TryDequeue(out var item))
            {
                try
                {
                    item.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(new InvalidOperationException($"Failed to dispose dependency '{item.GetType().Name}': {e.Message}", e));
                }
            }

            if (!exceptions.Any())
                return;

            if (exceptions.Count == 1)
                ExceptionDispatchInfo.Capture(exceptions[0]).Throw();

            throw new AggregateException("Failed to dispose dependencies", exceptions);
        }

        public IDependencyContainer BeginScope(Action<ContainerConfigurator>? configuration = null)
        {
            return new BasicDependencyContainer(this, configuration);
        }

        public override void RegisterInstance(object instance, RegistrationOptions options)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (!options.AsTypes.Any())
                options.As(instance.GetType());

            var instanceType = instance.GetType().GetTypeInfo();

            foreach (var asType in options.AsTypes)
            {
                if (!asType.GetTypeInfo().IsAssignableFrom(instanceType))
                    throw new InvalidOperationException($"Type {instanceType.AsType()} is not assignable to {asType}");

                _items.AddOrUpdate(asType, _ => instance, (_, __) => instance);
            }

            if (!options.IsExternallyOwned)
                EnlistDisposable(instance);
        }

        private object EnlistDisposable(object item)
        {
            if (item is IDisposable disposable)
                _disposable.Enqueue(disposable);
            return item;
        }
    }
}