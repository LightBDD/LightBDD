using System;
using System.Collections.Generic;
using System.Text;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results.Implementation
{
    internal class ScenarioResult : IScenarioResult
    {
        private IStepResult[] _steps = Array.Empty<IStepResult>();

        public ScenarioResult(IScenarioInfo info)
        {
            Info = info;
            Status = ExecutionStatus.NotRun;
        }

        public void UpdateResult(IStepResult[] steps, ExecutionTime executionTime)
        {
            _steps = steps;
            ExecutionTime = executionTime;
            CaptureStatus();
        }

        public IScenarioInfo Info { get; }
        public ExecutionStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public ExecutionTime ExecutionTime { get; private set; } = ExecutionTime.None;
        public Exception ExecutionException { get; private set; }

        public IEnumerable<IStepResult> GetSteps()
        {
            return _steps;
        }

        public void UpdateException(Exception exception)
        {
            ExecutionException = exception;
        }

        private void CaptureStatus()
        {
            var stepsResult = GetSteps().GetMostSevereOrNull();
            if (stepsResult == null)
                return;

            if (Status < stepsResult.Status)
                Status = stepsResult.Status;

            StatusDetails = GetSteps().MergeStatusDetails(StatusDetails);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Info);
            sb.Append(": ").Append(Status);
            if (StatusDetails != null)
                sb.Append(" (").Append(StatusDetails).Append(")");
            return sb.ToString();
        }

        public void UpdateScenarioResult(ExecutionStatus status, string details = null)
        {
            Status = status;
            if (!string.IsNullOrWhiteSpace(details))
                StatusDetails = $"Scenario: {details.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t")}";
        }
    }
}