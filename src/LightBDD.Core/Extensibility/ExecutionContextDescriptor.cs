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
        public static readonly ExecutionContextDescriptor NoContext = new ExecutionContextDescriptor(ProvideNoContext);

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExecutionContextDescriptor(Func<IDependencyResolver, Task<object>> contextResolver)
        {
            ContextResolver = contextResolver ?? throw new ArgumentNullException(nameof(contextResolver));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExecutionContextDescriptor(Func<object> contextProvider, bool takeOwnership)
            : this(res => res.RegisterInstance(contextProvider(), takeOwnership))
        {
        }

        /// <summary>
        /// Returns function providing execution context.
        /// </summary>
        [Obsolete("Use " + nameof(ContextResolver) + " instead", true)]
        public Func<object> ContextProvider => throw new NotSupportedException($"{nameof(ContextProvider)} is no longer supported");

        public Func<IDependencyResolver, Task<object>> ContextResolver { get; }
        /// <summary>
        /// Specifies if scenario/step runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after execution.
        /// </summary>
        [Obsolete]
        public bool TakeOwnership => throw new NotSupportedException($"{nameof(TakeOwnership)} is no longer supported");

        private static Task<object> ProvideNoContext(IDependencyResolver _)
        {
            return Task.FromResult<object>(null);
        }
    }
}