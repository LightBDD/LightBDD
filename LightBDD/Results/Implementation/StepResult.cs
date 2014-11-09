using System;
using System.Diagnostics;
using LightBDD.Naming;

namespace LightBDD.Results.Implementation
{
    [DebuggerStepThrough]
    internal class StepResult : IStepResult
    {
        public StepResult(int stepNumber, IStepName stepName, ResultStatus stepStatus, string statusDetails = null)
        {
            Number = stepNumber;
            StepName = stepName;
            Status = stepStatus;
            StatusDetails = statusDetails;
            Name = stepName.Format(StepNameDecorators.Default);
        }

        #region IStepResult Members

        public string Name { get; private set; }
        public ResultStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public TimeSpan? ExecutionTime { get; private set; }
        public DateTimeOffset? ExecutionStart { get; private set; }
        public IStepName StepName { get; private set; }
        public int Number { get; private set; }

        #endregion

        public StepResult SetExecutionTime(TimeSpan executionTime)
        {
            ExecutionTime = executionTime;
            return this;
        }

        public StepResult SetExecutionStart(DateTimeOffset executionStart)
        {
            ExecutionStart = executionStart;
            return this;
        }

        public StepResult SetStatus(ResultStatus status, string statusDetails = null)
        {
            Status = status;
            StatusDetails = statusDetails;
            return this;
        }

        public override string ToString()
        {
            var details = string.Empty;
            if (StatusDetails != null)
                details = string.Format(" ({0})", StatusDetails);

            return string.Format("{0} {1}: {2}{3}", Number, Name, Status, details);
        }
    }
}