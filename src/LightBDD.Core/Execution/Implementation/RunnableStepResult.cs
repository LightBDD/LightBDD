using System.Diagnostics;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class RunnableStepResult
    {
        public RunnableStepResult(RunnableStep[] subSteps)
        {
            SubSteps = subSteps;
        }

        public RunnableStep[] SubSteps { get; }
    }
}