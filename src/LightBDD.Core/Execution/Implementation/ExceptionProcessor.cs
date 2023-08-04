#nullable enable
using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ExceptionProcessor
    {
        private readonly Func<Exception, ExecutionStatus> _exceptionToStatusMapper;
        private readonly Func<Exception, string> _exceptionFormatter;

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
    }
}