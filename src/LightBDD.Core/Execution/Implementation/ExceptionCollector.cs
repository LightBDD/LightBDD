#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ExceptionCollector
    {
        private readonly ConcurrentQueue<Exception> _executionExceptions = new();

        public void Capture(Exception exception)
        {
            _executionExceptions.Enqueue(exception is ScenarioExecutionException ? exception.InnerException! : exception);
        }

        public Exception? CollectFor(ExecutionStatus executionStatus, IEnumerable<IStepResult> subSteps)
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

        public async Task Capture(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                _executionExceptions.Enqueue(ex);
            }
        }

        public void Capture(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _executionExceptions.Enqueue(ex);
            }
        }

        public Exception? Collect()
        {
            var exceptions = _executionExceptions.ToArray();
            return exceptions.Length switch
            {
                0 => null,
                1 => exceptions[0],
                _ => new AggregateException(exceptions)
            };
        }
    }
}