using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LightBDD.Formatters;

namespace LightBDD.Results.Implementation
{
    [DebuggerStepThrough]
    internal class ScenarioResult : IScenarioResult
    {
        public ScenarioResult(string scenarioName, IEnumerable<IStepResult> steps, string label)
        {
            Name = scenarioName;
            Steps = steps.ToArray();
            Label = label;

            SetStatus();
        }

        #region IScenarioResult Members

        public string Name { get; private set; }
        public ResultStatus Status { get; private set; }
        public IEnumerable<IStepResult> Steps { get; private set; }
        public TimeSpan? ExecutionTime { get; private set; }
        public DateTimeOffset? ExecutionStart { get; private set; }
        public string StatusDetails { get; private set; }
        public string Label { get; private set; }

        #endregion

        private void SetStatus()
        {
            var stepsResult = Steps.OrderByDescending(s => s.Status).FirstOrDefault();
            if (stepsResult == null)
                return;
            Status = stepsResult.Status;
            StatusDetails = stepsResult.StatusDetails;
        }

        public ScenarioResult SetExecutionTime(TimeSpan executionTime)
        {
            ExecutionTime = executionTime;
            return this;
        }

        public ScenarioResult SetExecutionStart(DateTimeOffset executionStart)
        {
            ExecutionStart = executionStart;
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            if (!string.IsNullOrWhiteSpace(Label))
                sb.Append(" [").Append(Label).Append("]");
            sb.Append(": ").Append(Status);
            if (StatusDetails != null)
                sb.Append(" (").Append(StatusDetails).Append(")");
            return sb.ToString();
        }
    }
}