using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Dependencies;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Class describing execution context for contextual scenarios or steps.
    /// </summary>
    [DebuggerStepThrough]
    public class ExecutionContextDescriptor
    {
        /// <summary>
        /// No context descriptor.
        /// </summary>
        public static readonly ExecutionContextDescriptor NoContext = new ExecutionContextDescriptor(ProvideNoContext, null);
        /// <summary>
        /// Returns function providing execution context.
        /// </summary>
        [Obsolete("Use " + nameof(ContextResolver) + " instead", true)]
        public Func<object> ContextProvider => throw new NotSupportedException($"{nameof(ContextProvider)} is no longer supported");


        public Action<IContainerConfigurer> ScopeConfigurer { get; }
        public Func<IDependencyResolver, Task<object>> ContextResolver { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExecutionContextDescriptor(Func<IDependencyResolver, Task<object>> contextResolver, Action<IContainerConfigurer> scopeConfigurer)
        {
            ScopeConfigurer = scopeConfigurer;
            ContextResolver = contextResolver ?? throw new ArgumentNullException(nameof(contextResolver));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExecutionContextDescriptor(Func<object> contextProvider, bool takeOwnership)
        {
            ScopeConfigurer = cfg => cfg.RegisterInstance(new ContextWrapper(contextProvider, takeOwnership), RegistrationOptions.Default);
            ContextResolver = ResolveContextWrapper;
        }

        private static async Task<object> ResolveContextWrapper(IDependencyResolver resolver)
        {
            return (await resolver.ResolveAsync<ContextWrapper>()).GetContext();
        }

        /// <summary>
        /// Specifies if scenario/step runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after execution.
        /// </summary>
        [Obsolete]
        public bool TakeOwnership => throw new NotSupportedException($"{nameof(TakeOwnership)} is no longer supported");

        private static Task<object> ProvideNoContext(IDependencyResolver _)
        {
            return Task.FromResult<object>(null);
        }

        [DebuggerStepThrough]
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
                return _instance ?? (_instance = _contextProvider());
            }
        }
    }
}