using System;
using System.Diagnostics;

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
        public static readonly ExecutionContextDescriptor NoContext = new ExecutionContextDescriptor(ProvideNoContext, false);

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExecutionContextDescriptor(Func<object> contextProvider, bool takeOwnership)
        {
            ContextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            TakeOwnership = takeOwnership;
        }

        /// <summary>
        /// Returns function providing execution context.
        /// </summary>
        public Func<object> ContextProvider { get; }
        /// <summary>
        /// Specifies if scenario/step runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after execution.
        /// </summary>
        public bool TakeOwnership { get; }

        private static object ProvideNoContext()
        {
            return null;
        }
    }
}