using System;
using System.Diagnostics;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]//TODO: delete
    internal class CompositeStepContext : IDisposable
    {
        private readonly IDependencyContainer _subStepContainer;
        public static readonly CompositeStepContext Empty = new CompositeStepContext(null, new RunnableStep[0]);

        public CompositeStepContext(IDependencyContainer subStepContainer, RunnableStep[] subSteps)
        {
            _subStepContainer = subStepContainer;
            SubSteps = subSteps;
        }

        public RunnableStep[] SubSteps { get; }

        public void Dispose()
        {
            _subStepContainer?.Dispose();
        }
    }
}