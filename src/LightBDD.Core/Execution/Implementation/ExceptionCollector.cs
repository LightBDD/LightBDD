using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ExceptionCollector
    {
        private readonly List<Exception> _executionExceptions = new();

        public void Capture(Exception exception)
        {
            _executionExceptions.Add(exception is ScenarioExecutionException ? exception.InnerException : exception);
        }

        public Exception CollectFor(ExecutionStatus executionStatus, IEnumerable<IStepResult> subSteps)
        {
            if (executionStatus < ExecutionStatus.Ignored)
                return null;

            var exceptions = _executionExceptions.Concat(
                    subSteps
                        .Where(s => s.Status == executionStatus)
                        .Select(s => s.ExecutionException)
                        .Where(x => x != null))
                .ToArray();

            return executionStatus == ExecutionStatus.Ignored || exceptions.Length == 1
                ? exceptions.First()
                : new AggregateException(exceptions);
        }
    }
}