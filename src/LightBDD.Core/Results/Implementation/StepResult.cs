using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Results.Implementation
{
    internal class StepResult : IStepResult
    {
        private readonly StepInfo _info;
        private readonly ConcurrentQueue<string> _comments = new ConcurrentQueue<string>();
        private IEnumerable<IStepResult> _subSteps = Array.Empty<IStepResult>();

        public StepResult(StepInfo info, MethodArgument[] parameters)
        {
            _info = info;
            Parameters = parameters;
            _info.UpdateName(parameters);
        }

        public IStepInfo Info => _info;
        public ExecutionStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public Exception ExecutionException { get; private set; }
        public IReadOnlyList<IParameterResult> Parameters { get; }
        public ExecutionTime ExecutionTime { get; private set; }
        public IEnumerable<string> Comments => _comments;
        public IEnumerable<IStepResult> GetSubSteps()
        {
            return _subSteps;
        }

        public void SetStatus(ExecutionStatus status, string details = null)
        {
            Status = status;
            if (!string.IsNullOrWhiteSpace(details))
                StatusDetails = $"Step {_info.GroupPrefix}{_info.Number}: {details.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t")}";
        }

        public void UpdateException(Exception exception)
        {
            ExecutionException = exception;
        }

        public void SetExecutionTime(ExecutionTime executionTime)
        {
            ExecutionTime = executionTime;
        }

        public void AddComment(string comment)
        {
            _comments.Enqueue(comment);
        }

        public override string ToString()
        {
            var details = string.Empty;
            if (StatusDetails != null)
                details = $" ({StatusDetails})";

            return $"{Info}: {Status}{details}";
        }

        public void SetSubSteps(IStepResult[] subSteps)
        {
            _subSteps = subSteps;
        }

        public void IncludeSubStepDetails()
        {
            StatusDetails = GetSubSteps().MergeStatusDetails(StatusDetails);
        }
    }
}