using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
    }
}