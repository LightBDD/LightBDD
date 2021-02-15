using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Dependencies.Implementation
{
    class DependencyFactory : ContainerConfigurator, IDefaultContainerConfigurator
    {
        private readonly List<DependencyDescriptor> _descriptors = new List<DependencyDescriptor>();

        public IReadOnlyList<DependencyDescriptor> Descriptors => _descriptors;

        public void RegisterSingleton(object instance, Action<RegistrationOptions> options = null)
        {
            if (instance is null) throw new ArgumentNullException(nameof(instance));
            Register(instance.GetType(), _ => instance, options, InstanceScope.Single, true);
        }

        public override void RegisterInstance(object instance, RegistrationOptions options)
        {
            if (instance is null) throw new ArgumentNullException(nameof(instance));
            _descriptors.Add(new DependencyDescriptor(instance.GetType(), _ => instance, options, InstanceScope.Single, true));
        }

        public void RegisterSingleton<T>(Action<RegistrationOptions> options = null)
        {
            Register(typeof(T), DependencyDescriptor.FindConstructor(typeof(T)), options, InstanceScope.Single, false);
        }

        public void RegisterSingleton<T>(Func<IDependencyResolver, T> createFn, Action<RegistrationOptions> options = null)
        {
            Register(typeof(T), r => createFn(r), options, InstanceScope.Single, false);
        }

        public void RegisterTransient<T>(Action<RegistrationOptions> options = null)
        {
            Register(typeof(T), DependencyDescriptor.FindConstructor(typeof(T)), options, InstanceScope.Transient, false);
        }

        public void RegisterTransient<T>(Func<IDependencyResolver, T> createFn, Action<RegistrationOptions> options = null)
        {
            Register(typeof(T), r => createFn(r), options, InstanceScope.Transient, false);
        }

        public void RegisterScenarioScoped<T>(Action<RegistrationOptions> options = null)
        {
            Register(typeof(T), DependencyDescriptor.FindConstructor(typeof(T)), options, InstanceScope.Scenario, false);
        }

        public void RegisterScenarioScoped<T>(Func<IDependencyResolver, T> createFn, Action<RegistrationOptions> options = null)
        {
            Register(typeof(T), r => createFn(r), options, InstanceScope.Scenario, false);
        }

        public void RegisterLocallyScoped<T>(Func<IDependencyResolver, T> createFn, Action<RegistrationOptions> options = null)
        {
            Register(typeof(T), r => createFn(r), options, InstanceScope.Local, false);
        }

        public void RegisterLocallyScoped<T>(Action<RegistrationOptions> options = null)
        {
            Register(typeof(T), DependencyDescriptor.FindConstructor(typeof(T)), options, InstanceScope.Local, false);
        }

        private void Register(Type type, Func<IDependencyResolver, object> resolveFn, Action<RegistrationOptions> optionsFn, InstanceScope scope, bool instantResolution)
        {
            var registration = new RegistrationOptions();
            optionsFn?.Invoke(registration);
            _descriptors.Add(new DependencyDescriptor(type, resolveFn, registration, scope, instantResolution));
        }
    }
}