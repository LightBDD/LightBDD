#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
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
        public static readonly ExecutionContextDescriptor NoContext = new(ProvideNoContext);

        /// <summary>
        /// Returns context resolver function used to create context.
        /// </summary>
        public Func<IDependencyResolver, object?> ContextResolver { get; }

        /// <summary>
        /// Constructor allowing to configure descriptor with <paramref name="contextResolver"/>.
        /// </summary>
        public ExecutionContextDescriptor(Func<IDependencyResolver, object?> contextResolver)
        {
            ContextResolver = contextResolver ?? throw new ArgumentNullException(nameof(contextResolver));
        }

        /// <summary>
        /// Constructor allowing to configure descriptor with <paramref name="contextProvider"/> delegate and <paramref name="takeOwnership"/> which specifies if instance should be disposed by the container if implements <see cref="IDisposable"/> or <see cref="IAsyncDisposable"/> interfaces.
        /// </summary>
        public ExecutionContextDescriptor(Func<object?> contextProvider, bool takeOwnership)
        {
            if (!takeOwnership)
                ContextResolver = _ => contextProvider();
            else
                ContextResolver = r => r.Resolve<ContextWrapper>().WithInstance(contextProvider());
        }

        private static object? ProvideNoContext(IDependencyResolver _) => null;

        private class ContextWrapper : IDisposable, IAsyncDisposable
        {
            private object? _instance;
            public object? WithInstance(object? instance) => _instance = instance;

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _instance, null) is not IDisposable disposable)
                    return;

                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Failed to dispose context '{disposable.GetType().Name}': {e.Message}", e);
                }
            }

            //TODO: test
            public async ValueTask DisposeAsync()
            {
                if (Interlocked.Exchange(ref _instance, null) is not IAsyncDisposable disposable)
                    return;

                try
                {
                    await disposable.DisposeAsync();
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Failed to dispose context '{disposable.GetType().Name}': {e.Message}", e);
                }
            }
        }
    }
}