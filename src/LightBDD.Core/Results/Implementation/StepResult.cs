using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Results.Implementation
{
    internal class StepResult : IStepResult
    {
        private readonly StepInfo _info;
        private readonly ConcurrentQueue<string> _comments = new();
        private readonly ConcurrentQueue<FileAttachment> _attachments = new();
        private IEnumerable<IStepResult> _subSteps = Array.Empty<IStepResult>();

        public StepResult(StepInfo info)
        {
            _info = info;
        }

        public IStepInfo Info => _info;
        public ExecutionStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public Exception ExecutionException { get; private set; }
        public IReadOnlyList<IParameterResult> Parameters { get; private set; } = Array.Empty<IParameterResult>();
        public ExecutionTime ExecutionTime { get; private set; }
        public IEnumerable<string> Comments => _comments;
        public IEnumerable<IStepResult> GetSubSteps() => _subSteps;
        public IEnumerable<FileAttachment> FileAttachments => _attachments;

        public void SetStatus(ExecutionStatus status, string details = null)
        {
            Status = status;
            StatusDetails = !string.IsNullOrWhiteSpace(details)
                ? $"Step {_info.GroupPrefix}{_info.Number}: {details.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t")}"
                : null;
        }

        public void UpdateException(Exception exception)
        {
            ExecutionException = exception;
        }

        public void UpdateName(INameParameterInfo[] parameters)
        {
            _info.UpdateName(parameters);
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

        public void SetParameters(IEnumerable<IParameterResult> parameters)
        {
            Parameters = parameters.ToArray();
        }

        public void AddAttachment(FileAttachment attachment)
        {
            _attachments.Enqueue(attachment);
        }
    }
}