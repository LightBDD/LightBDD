using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class ExceptionProcessor
    {
        private readonly Func<Exception, ExecutionStatus> _exceptionToStatusMapper;
        private readonly Func<Exception, string> _exceptionFormatter;

        public ExceptionProcessor(IntegrationContext integrationContext)
        {
            _exceptionToStatusMapper = integrationContext.ExceptionToStatusMapper;
            _exceptionFormatter = integrationContext.Configuration.ExceptionHandlingConfiguration().ExceptionDetailsFormatter;
        }

        public ExecutionStatus UpdateResultsWithException(StepResult result, Exception exception)
        {
            var status = _exceptionToStatusMapper.Invoke(exception);
            var details = (status == ExecutionStatus.Failed) ? _exceptionFormatter.Invoke(exception) : exception.Message;
            result.SetStatus(status, details);
            return status;
        }
    }
}