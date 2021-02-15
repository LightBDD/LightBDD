using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace LightBDD.Core.Dependencies.Implementation
{
    internal class DefaultDependencyContainer : IDependencyContainerV2
    {
        class Slot
        {
            public readonly SemaphoreSlim Lock = new SemaphoreSlim(1);
            public readonly DependencyDescriptor Descriptor;
            public object Instance;

            public Slot(DependencyDescriptor descriptor)
            {
                Descriptor = descriptor;
            }
        }

        private readonly DependencyFactory _dependencyFactory = new DependencyFactory();
        private readonly DefaultDependencyContainer _parent;
        private readonly LifetimeScope _scope;
        private readonly ConcurrentQueue<IDisposable> _disposable = new ConcurrentQueue<IDisposable>();
        private readonly ConcurrentDictionary<Type, Slot> _items = new ConcurrentDictionary<Type, Slot>();

        public DefaultDependencyContainer(LifetimeScope scope, Action<DependencyFactory> configuration = null) : this(null, scope, configuration) { }
        private DefaultDependencyContainer(DefaultDependencyContainer parent, LifetimeScope scope, Action<DependencyFactory> configuration = null)
        {
            _parent = parent;
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            configuration?.Invoke(_dependencyFactory);

            foreach (var descriptor in GetTraversableDescriptors())
                InitializeSlots(descriptor);
        }

        private IEnumerable<DependencyDescriptor> GetTraversableDescriptors()
        {
            var parent = _parent?.GetTraversableDescriptors().Where(_parent.IsInheritable)
                         ?? Enumerable.Empty<DependencyDescriptor>();

            return parent.Concat(_dependencyFactory.Descriptors);
        }

        private bool IsInheritable(DependencyDescriptor descriptor)
        {
            if (!descriptor.Scope.LifetimeScopeRestriction?.Equals(_scope) ?? false)
                return true;
            return !descriptor.Scope.IsSharedWithNestedScopes;
        }

        private void InitializeSlots(DependencyDescriptor descriptor)
        {
            if (!IsValidScope(descriptor))
                return;

            var slot = new Slot(descriptor);
            foreach (var type in slot.Descriptor.Registration.AsTypes)
            {
                if (!type.IsAssignableFrom(descriptor.Type))
                    throw new InvalidOperationException($"Type {descriptor.Type} is not assignable to {type}");
                _items[type] = slot;
            }
        }

        private bool IsValidScope(DependencyDescriptor descriptor)
        {
            return descriptor.Scope.LifetimeScopeRestriction?.Equals(_scope) ?? true;
        }

        public object Resolve(Type type)
        {
            try
            {
                if (type == typeof(IDependencyContainer) || type == typeof(IDependencyResolver))
                    return this;

                if (ResolveMapped(this, type, out var cached))
                    return cached;

                return EnlistDisposable(DependencyDescriptor.FindConstructor(type).Invoke(this));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Unable to resolve type {type}:{Environment.NewLine}{e.Message}", e);
            }
        }

        private static bool ResolveMapped(DefaultDependencyContainer container, Type type, out object cached)
        {
            cached = null;

            do
            {
                if (container._items.TryGetValue(type, out var slot))
                {
                    cached = container.ResolveSlotted(slot);
                    return true;
                }

                container = container._parent;
            } while (container != null);

            return false;
        }

        private object ResolveSlotted(Slot slot)
        {
            if (slot.Instance != null)
                return slot.Instance;

            var descriptor = slot.Descriptor;

            if (!descriptor.Scope.IsSharedInstance)
                return InstantiateDependency(descriptor);

            if (!slot.Lock.Wait(TimeSpan.FromSeconds(10)))
                throw new InvalidOperationException($"Unable to resolve {descriptor.Type} due to potential circular dependency");

            try
            {
                if (slot.Instance != null)
                    return slot.Instance;

                return slot.Instance = InstantiateDependency(descriptor);
            }
            finally
            {
                slot.Lock.Release();
            }
        }

        private object InstantiateDependency(DependencyDescriptor descriptor)
        {
            var instance = descriptor.ResolveFn(this);
            if (!descriptor.Registration.IsExternallyOwned)
                return EnlistDisposable(instance);
            return instance;
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

        public IDependencyContainer BeginScope(Action<ContainerConfigurator> configuration = null) =>
            BeginScope(LifetimeScope.Local, configuration);

        public IDependencyContainerV2 BeginScope(LifetimeScope scope, Action<ContainerConfigurator> configuration = null)
        {
            return new DefaultDependencyContainer(this, scope, configuration);
        }

        private object EnlistDisposable(object item)
        {
            if (item is IDisposable disposable)
                _disposable.Enqueue(disposable);
            return item;
        }
    }
}