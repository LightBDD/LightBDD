namespace LightBDD.Results.Implementation
{
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
        public int Number { get; private set; }

        #endregion

        public void SetStatus(ResultStatus status, string statusDetails = null)
        {
            Status = status;
            StatusDetails = statusDetails;
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