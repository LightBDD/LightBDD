using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace LightBDD.Core.Dependencies.Implementation
{
    internal class BasicDependencyContainer : IDependencyContainer, IContainerConfigurer
    {
        private readonly BasicDependencyContainer _parent;
        private readonly ConcurrentQueue<IDisposable> _disposable = new ConcurrentQueue<IDisposable>();
        private readonly ConcurrentDictionary<Type, object> _items = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, Func<object>> _ctorCache = new ConcurrentDictionary<Type, Func<object>>();

        public BasicDependencyContainer(Action<IContainerConfigurer> configuration = null) : this(null, configuration) { }
        private BasicDependencyContainer(BasicDependencyContainer parent, Action<IContainerConfigurer> configuration = null)
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
            do
            {
                if (container._items.TryGetValue(type, out cached))
                    return true;

                container = container._parent;
            } while (container != null);

            return false;
        }

        private object Create(Type type)
        {
            return _ctorCache.GetOrAdd(type, FindConstructor).Invoke();
        }

        private Func<object> FindConstructor(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var ctor = typeInfo.DeclaredConstructors.Where(x => x.IsPublic).OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();

            if (typeInfo.IsAbstract || (!typeInfo.IsClass && !typeInfo.IsValueType))
                return () => throw new InvalidOperationException($"Type '{type}' has to be non-abstract class or value type.");

            if (ctor == null)
                return () => throw new InvalidOperationException($"Type '{type}' does not have public constructor.");

            return () => ctor.Invoke(ctor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray());
        }

        public void Dispose()
        {
            while (_disposable.TryDequeue(out var item))
            {
                try
                {
                    item.Dispose();
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Failed to dispose dependency '{item.GetType().Name}': {e.Message}", e);
                }
            }
        }

        public IDependencyContainer BeginScope(Action<IContainerConfigurer> configuration = null)
        {
            return new BasicDependencyContainer(this, configuration);
        }

        void IContainerConfigurer.RegisterInstance(object instance, RegistrationOptions options)
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