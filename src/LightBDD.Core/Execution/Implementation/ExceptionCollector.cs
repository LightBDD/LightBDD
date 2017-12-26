using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class ExceptionCollector
    {
        private Exception _executionException;

        public void Capture(Exception exception)
        {
            _executionException = exception is ScenarioExecutionException ? exception.InnerException : exception;
        }

        public Exception CollectFor(ExecutionStatus executionStatus, IEnumerable<IStepResult> subSteps)
        {
            if (_executionException != null)
                return _executionException;

            if (executionStatus < ExecutionStatus.Ignored)
                return null;

            var exceptions = subSteps
                .Where(s => s.Status == executionStatus)
                .Select(s => s.ExecutionException)
                .Where(x => x != null).ToArray();

            return executionStatus == ExecutionStatus.Ignored || exceptions.Length == 1
                ? exceptions.First()
                : new AggregateException(exceptions);
        }
    }
}