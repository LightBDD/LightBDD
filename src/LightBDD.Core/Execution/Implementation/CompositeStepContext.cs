using System;
using System.Diagnostics;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class CompositeStepContext : IDisposable
    {
        public static readonly CompositeStepContext Empty = new CompositeStepContext(ContextProvider.NoContext, new RunnableStep[0]);

        public CompositeStepContext(IContextProvider contextProvider, RunnableStep[] subSteps)
        {
            Context = contextProvider;
            SubSteps = subSteps;
        }

        public IContextProvider Context { get; }
        public RunnableStep[] SubSteps { get; }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}