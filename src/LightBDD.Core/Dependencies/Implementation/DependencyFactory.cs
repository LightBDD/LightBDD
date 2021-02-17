using System;
using System.Collections.Generic;

namespace LightBDD.Core.Dependencies.Implementation
{
    class DependencyFactory : ContainerConfigurator, IDefaultContainerConfigurator
    {
        private readonly List<DependencyDescriptor> _descriptors = new List<DependencyDescriptor>();
        public IReadOnlyList<DependencyDescriptor> Descriptors => _descriptors;
        public FallbackResolveBehavior FallbackBehavior { get; private set; }

        public DependencyFactory(FallbackResolveBehavior fallbackBehavior)
        {
            FallbackBehavior = fallbackBehavior;
        }

        public void RegisterInstance(object instance, Action<RegistrationOptions>? options = null)
        {
            if (instance is null) throw new ArgumentNullException(nameof(instance));
            Register(instance.GetType(), _ => instance, options, InstanceScope.Single, true);
        }

        public override void RegisterInstance(object instance, RegistrationOptions options)
        {
            if (instance is null) throw new ArgumentNullException(nameof(instance));
            _descriptors.Add(new DependencyDescriptor(instance.GetType(), _ => instance, options, InstanceScope.Single, true));
        }

        private void Register(Type type, Func<IDependencyResolver, object?> resolveFn, Action<RegistrationOptions>? optionsFn, InstanceScope scope, bool instantResolution)
        {
            var registration = new RegistrationOptions();
            optionsFn?.Invoke(registration);
            _descriptors.Add(new DependencyDescriptor(type, resolveFn, registration, scope, instantResolution));
        }

        public void RegisterType<T>(InstanceScope scope, Action<RegistrationOptions>? options = null)
        {
            Register(typeof(T), DependencyDescriptor.FindConstructor(typeof(T)), options, scope, false);
        }

        public void RegisterType<T>(InstanceScope scope, Func<IDependencyResolver, T?> createFn, Action<RegistrationOptions>? options = null)
        {
            Register(typeof(T), r => createFn(r), options, scope, false);
        }

        public void ConfigureFallbackBehavior(FallbackResolveBehavior fallbackBehavior)
        {
            FallbackBehavior = fallbackBehavior;
        }
    }
}