using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Results.Implementation
{
    [DebuggerStepThrough]
    internal class StepResult : IStepResult
    {
        private readonly StepInfo _info;
        private readonly ConcurrentQueue<string> _comments = new ConcurrentQueue<string>();

        public StepResult(StepInfo info)
        {
            _info = info;
        }

        public IStepInfo Info => _info;
        public ExecutionStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public ExecutionTime ExecutionTime { get; private set; }
        public IEnumerable<string> Comments => _comments;
        public IEnumerable<IStepResult> SubSteps { get; private set; } = Arrays<IStepResult>.Empty();

        public void SetStatus(ExecutionStatus status, string details = null)
        {
            Status = status;
            StatusDetails = details;
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
                details = string.Format(" ({0})", StatusDetails);

            return $"{Info}: {Status}{details}";
        }

        public void SetSubSteps(IStepResult[] subSteps)
        {
            SubSteps = subSteps;
        }
    }
}