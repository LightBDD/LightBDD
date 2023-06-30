using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using LightBDD.Core.Dependencies;

namespace LightBDD.Extensions.DependencyInjection.Implementation
{
    internal class ContainerOverrides : ContainerConfigurator, IDisposable
    {
        private readonly ConcurrentStack<IDisposable> _disposables = new();
        private readonly ConcurrentDictionary<Type, object> _singletons = new();

        public override void RegisterInstance(object instance, RegistrationOptions options)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (!options.IsExternallyOwned && instance is IDisposable disposable)
                _disposables.Push(disposable);

            if (options.IsRegisteredAsSelf)
                _singletons.AddOrUpdate(instance.GetType(), instance, (_, __) => instance);

            foreach (var type in options.AsTypes)
                _singletons.AddOrUpdate(type, instance, (_, __) => instance);
        }

        public void Dispose()
        {
            var exceptions = new List<Exception>();

            foreach (var disposable in _disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(new InvalidOperationException($"Failed to dispose singleton dependency '{disposable.GetType().Name}': {e.Message}", e));
                }
            }

            if (!exceptions.Any())
                return;

            if (exceptions.Count == 1)
                ExceptionDispatchInfo.Capture(exceptions[0]).Throw();

            throw new AggregateException("Failed to dispose dependencies", exceptions);
        }

        public object TryResolve(Type type)
        {
            return _singletons.TryGetValue(type, out var instance)
                ? instance
                : null;
        }
    }
}