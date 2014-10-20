using System;
using System.Diagnostics;

namespace LightBDD.Results.Implementation
{
    [DebuggerStepThrough]
    internal class StepResult : IStepResult
    {
        public StepResult(int stepNumber, string stepName, ResultStatus stepStatus, string statusDetails = null)
        {
            Number = stepNumber;
            Name = stepName;
            Status = stepStatus;
            StatusDetails = statusDetails;
        }

        #region IStepResult Members

        public string Name { get; private set; }
        public ResultStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public TimeSpan? ExecutionTime { get; private set; }
        public DateTimeOffset? ExecutionStart { get; private set; }
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

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj)
                && (ReferenceEquals(this, obj)
                    || (obj.GetType() == GetType() && Equals((StepResult)obj)));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Status;
                hashCode = (hashCode * 397) ^ (StatusDetails != null ? StatusDetails.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Number;
                return hashCode;
            }
        }

        public override string ToString()
        {
            var details = string.Empty;
            if (StatusDetails != null)
                details = string.Format(" ({0})", StatusDetails);

            return string.Format("{0} {1}: {2}{3}", Number, Name, Status, details);
        }

        protected bool Equals(StepResult other)
        {
            return string.Equals(Name, other.Name)
                   && Status == other.Status
                   && string.Equals(StatusDetails, other.StatusDetails)
                   && Number == other.Number;
        }
    }
}