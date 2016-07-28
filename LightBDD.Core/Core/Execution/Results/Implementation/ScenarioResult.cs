using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution.Results.Implementation
{
    internal class ScenarioResult : IScenarioResult
    {
        private readonly IStepResult[] _steps;

        public ScenarioResult(IScenarioInfo info, IStepResult[] steps, ExecutionTime executionTime, Exception scenarioInitializationException)
        {
            Info = info;
            _steps = steps;
            ExecutionTime = executionTime;
            CaptureStatus(scenarioInitializationException);
        }

        public IScenarioInfo Info { get; }
        public ExecutionStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public ExecutionTime ExecutionTime { get; }

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
    }
}