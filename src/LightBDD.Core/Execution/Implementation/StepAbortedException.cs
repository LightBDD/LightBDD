using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class StepAbortedException : Exception
    {
        public ExecutionStatus StepStatus { get; }

        public StepAbortedException(Exception inner, ExecutionStatus stepStatus)
            : base("Step failed to execute", inner)
        {
            StepStatus = stepStatus;
        }

        public void RethrowOriginalException()
        {
            ExceptionDispatchInfo.Capture(InnerException).Throw();
        }
    }
}