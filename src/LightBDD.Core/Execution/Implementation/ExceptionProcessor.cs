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
        {
            _exceptionToStatusMapper = integrationContext.ExceptionToStatusMapper;
            _exceptionFormatter = integrationContext.Configuration.ExceptionHandlingConfiguration().ExceptionDetailsFormatter;
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