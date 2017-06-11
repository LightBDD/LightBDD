using System.Diagnostics;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class RunnableStepResult
    {
        public static readonly RunnableStepResult Empty = new RunnableStepResult(new RunnableStep[0]);

        public RunnableStepResult(RunnableStep[] subSteps)
        {
            SubSteps = subSteps;
        }

        public RunnableStep[] SubSteps { get; }
    }
}