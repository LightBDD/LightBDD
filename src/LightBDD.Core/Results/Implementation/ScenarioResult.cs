using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results.Implementation
{
    [DebuggerStepThrough]
    internal class ScenarioResult : IScenarioResult
    {
        private IStepResult[] _steps = Arrays<IStepResult>.Empty();

        public ScenarioResult(IScenarioInfo info)
        {
            Info = info;
            Status = ExecutionStatus.NotRun;
        }

        public void UpdateResult(IStepResult[] steps, ExecutionTime executionTime, Exception scenarioInitializationException)
        {
            _steps = steps;
            ExecutionTime = executionTime;
            CaptureStatus(scenarioInitializationException);
        }

        public IScenarioInfo Info { get; }
        public ExecutionStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public ExecutionTime ExecutionTime { get; private set; }

        public IEnumerable<IStepResult> GetSteps() => _steps;

        private void CaptureStatus(Exception scenarioInitializationException)
        {
            if (scenarioInitializationException != null)
            {
                Status = ExecutionStatus.Failed;
                StatusDetails = scenarioInitializationException.Message;
                return;
            }

            var stepsResult = GetSteps().Reverse().OrderByDescending(s => s.Status).FirstOrDefault();
            if (stepsResult == null)
                return;
            Status = stepsResult.Status;
            StatusDetails = CaptureStatusDetails();
        }

        private string CaptureStatusDetails()
        {
            var sb = new StringBuilder();
            foreach (var step in GetSteps().Where(s => s.StatusDetails != null))
            {
                if (sb.Length > 0)
                    sb.AppendLine();

                sb.Append("Step ")
                    .Append(step.Info.Number)
                    .Append(": ")
                    .Append(step.StatusDetails.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t"));
            }
            return sb.Length > 0 ? sb.ToString() : null;
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
    }
}