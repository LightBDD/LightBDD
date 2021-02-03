using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using LightBDD.Core.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Extensions.DependencyInjection.Implementation
{
    internal class DiContainer : IDependencyContainerV2
    {
        private readonly IServiceScope _scope;
        private readonly ContainerOverrides _overrides;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public DiContainer(IServiceScope scope, ContainerOverrides overrides)
        {
            overrides.RegisterInstance(this, new RegistrationOptions().ExternallyOwned().As<IDependencyContainer>().As<IDependencyResolver>());

            _scope = scope;
            _overrides = overrides;
            AddDisposable(_scope);
        }

        public object Resolve(Type type)
        {
            return _overrides.TryResolve(type) ?? _scope.ServiceProvider.GetRequiredService(type);
        }

        public void Dispose()
        {
            var exceptions = new List<Exception>();

            try { _overrides.Dispose(); }
            catch (Exception e) { exceptions.Add(e); }

            foreach (var disposable in _disposables)
            {
                try { disposable.Dispose(); }
                catch (Exception e) { exceptions.Add(e); }
            }

            if (!exceptions.Any())
                return;

            if (exceptions.Count == 1)
                ExceptionDispatchInfo.Capture(exceptions[0]).Throw();

            throw new AggregateException(exceptions);
        }

        public IDependencyContainer BeginScope(Action<ContainerConfigurator> configuration = null) =>
            BeginScope(LifetimeScope.Local, configuration);

        public IDependencyContainerV2 BeginScope(LifetimeScope scope, Action<ContainerConfigurator> configuration = null)
        {
            var overrides = new ContainerOverrides();
            configuration?.Invoke(overrides);

            return new DiContainer(_scope.ServiceProvider.CreateScope(), overrides);
        }

        public void AddDisposable(IDisposable serviceProvider)
        {
            _disposables.Add(serviceProvider);
        }
    }
}