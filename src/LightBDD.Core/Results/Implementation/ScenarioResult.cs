#nullable enable
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
        public IScenarioInfo Info { get; }
        public ExecutionStatus Status { get; private set; }
        public string? StatusDetails { get; private set; }
        public ExecutionTime ExecutionTime { get; private set; } = ExecutionTime.None;
        public Exception? ExecutionException { get; private set; }


        public ScenarioResult(IScenarioInfo info)
        {
            Info = info;
            Status = ExecutionStatus.NotRun;
        }

        public void UpdateResult(IStepResult[] steps, ExecutionTime executionTime)
        {
            UpdateResults(steps);
            UpdateTime(executionTime);
        }

        public void UpdateTime(ExecutionTime executionTime)
        {
            ExecutionTime = executionTime;
        }

        public void UpdateResults(IStepResult[] steps)
        {
            _steps = steps;
            CaptureStatus();
        }

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

        public void UpdateScenarioResult(ExecutionStatus status, string? details = null)
        {
            Status = status;
            if (!string.IsNullOrWhiteSpace(details))
                StatusDetails = $"Scenario: {details!.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t")}";
        }

        public void UpdateScenarioResultV2(ExecutionStatus status, string? details = null, Exception? executionException = null)
        {
            Status = status;
            ExecutionException = executionException;
            if (!string.IsNullOrWhiteSpace(details))
            {
                var formattedDetails = $"Scenario {Status}: {details!.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t")}";
                StatusDetails = string.IsNullOrWhiteSpace(StatusDetails)
                    ? formattedDetails
                    : $"{formattedDetails}{Environment.NewLine}{StatusDetails}";
            }
        }

        public static ScenarioResult CreateFailed(IScenarioInfo scenarioInfo, Exception ex)
        {
            var result = new ScenarioResult(scenarioInfo);
            result.UpdateException(ex);
            result.UpdateScenarioResult(ExecutionStatus.Failed, ex.Message);
            return result;
        }

        public static ScenarioResult CreateIgnored(IScenarioInfo scenarioInfo, string message)
        {
            var result = new ScenarioResult(scenarioInfo);
            result.UpdateScenarioResult(ExecutionStatus.Ignored, message);
            return result;
        }
    }
}