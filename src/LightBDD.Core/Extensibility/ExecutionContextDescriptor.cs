using System;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Class describing execution context for contextual scenarios or steps.
    /// </summary>
    public class ExecutionContextDescriptor
    {
        /// <summary>
        /// No context descriptor.
        /// </summary>
        public static readonly ExecutionContextDescriptor NoContext = new(ProvideNoContext, null);

        /// <summary>
        /// Returns container configurator function used to configure container used in the execution context scope.
        /// </summary>
        public Action<ContainerConfigurator> ScopeConfigurator { get; }
        /// <summary>
        /// Returns context resolver function used to create context.
        /// </summary>
        public Func<IDependencyResolver, object> ContextResolver { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExecutionContextDescriptor(Func<IDependencyResolver, object> contextResolver, Action<ContainerConfigurator> scopeConfigurator)
        {
            ScopeConfigurator = scopeConfigurator;
            ContextResolver = contextResolver ?? throw new ArgumentNullException(nameof(contextResolver));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExecutionContextDescriptor(Func<object> contextProvider, bool takeOwnership)
        {
            var options = new RegistrationOptions().As<ContextWrapper>();
            if (!takeOwnership)
                options.ExternallyOwned();

            ScopeConfigurator = cfg => cfg.RegisterInstance(new ContextWrapper(contextProvider, takeOwnership), options);
            ContextResolver = ResolveContextWrapper;
        }

        private static object ResolveContextWrapper(IDependencyResolver resolver) => resolver.Resolve<ContextWrapper>().GetContext();

        private static object ProvideNoContext(IDependencyResolver _) => null;

        private class ContextWrapper : IDisposable
        {
            private readonly Func<object> _contextProvider;
            private readonly bool _takeOwnership;
            private object _instance;

            public ContextWrapper(Func<object> contextProvider, bool takeOwnership)
            {
                _contextProvider = contextProvider;
                _takeOwnership = takeOwnership;
            }

            public void Dispose()
            {
                if (!_takeOwnership || !(_instance is IDisposable disposable))
                    return;

                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Failed to dispose context '{_instance.GetType().Name}': {e.Message}", e);
                }
            }

            public object GetContext()
            {
                return _instance ??= _contextProvider();
            }
        }
    }
}