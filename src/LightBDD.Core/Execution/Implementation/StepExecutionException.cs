using System;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class StepExecutionException : Exception
    {
        public ExecutionStatus StepStatus { get; }

        public StepExecutionException(Exception inner, ExecutionStatus stepStatus)
            : base("Step failed to execute", inner)
        {
            StepStatus = stepStatus;
        }
    }
}