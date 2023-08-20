#nullable enable
using System;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ExceptionProcessor
    {
        private readonly Func<Exception, ExecutionStatus> _exceptionToStatusMapper;
        private readonly IExceptionFormatter _exceptionFormatter;

        public ExceptionProcessor(IExceptionFormatter exceptionFormatter)
        {
            _exceptionFormatter = exceptionFormatter;
            _exceptionToStatusMapper = ex => ex is IgnoreException ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
        }

        public ExecutionStatus UpdateResultsWithException(Action<ExecutionStatus, string> setStatus, Exception exception)
        {
            var status = _exceptionToStatusMapper.Invoke(exception);
            var details = status == ExecutionStatus.Failed ? _exceptionFormatter.Format(exception) : exception.Message;
            setStatus(status, details);
            return status;
        }
    }
}