#nullable enable
using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ExceptionProcessor
    {
        private readonly Func<Exception, ExecutionStatus> _exceptionToStatusMapper;
        private readonly Func<Exception, string> _exceptionFormatter;

        public ExceptionProcessor(IntegrationContext integrationContext)
        : this(integrationContext.Configuration)
        {
            _exceptionToStatusMapper = integrationContext.ExceptionToStatusMapper;
        }

        public ExceptionProcessor(LightBddConfiguration configuration)
        {
            _exceptionFormatter = configuration.ExceptionHandlingConfiguration().ExceptionDetailsFormatter;
            _exceptionToStatusMapper = ex => ex is IgnoreException ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
        }

        public ExecutionStatus UpdateResultsWithException(Action<ExecutionStatus, string> setStatus, Exception exception)
        {
            var status = _exceptionToStatusMapper.Invoke(exception);
            var details = status == ExecutionStatus.Failed ? _exceptionFormatter.Invoke(exception) : exception.Message;
            setStatus(status, details);
            return status;
        }

        public static void UpdateStatus(Action<ExecutionStatus, string?, Exception?> updateStatusFn, Exception exception, LightBddConfiguration cfg)
        {
            if (exception is StepExecutionException ste)
            {
                updateStatusFn(ste.StepStatus, null, ste.InnerException);
                return;
            }
            if (exception is ScenarioExecutionException e)
                exception = e.InnerException!;

            var status = MapStatus(exception);
            if (status == ExecutionStatus.Failed)
                updateStatusFn(status, cfg.ExceptionHandlingConfiguration().ExceptionDetailsFormatter(exception), exception);
            else
                updateStatusFn(status, exception.Message, null);
        }

        private static ExecutionStatus MapStatus(Exception exception)
        {
            return exception switch
            {
                BypassException => ExecutionStatus.Bypassed,
                IgnoreException => ExecutionStatus.Ignored,
                _ => ExecutionStatus.Failed
            };
        }
    }
}