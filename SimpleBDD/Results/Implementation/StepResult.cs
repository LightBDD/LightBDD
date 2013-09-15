namespace SimpleBDD.Results.Implementation
{
	internal class StepResult : IStepResult
	{
		public string Name { get; set; }
		public ResultStatus Status { get; set; }
		public int StepNumber { get; set; }
		public int TotalStepsCount { get; set; }
		public StepResult(int stepNumber, int totalStepsCount, string stepName, ResultStatus stepStatus)
		{
			StepNumber = stepNumber;
			TotalStepsCount = totalStepsCount;
			Name = stepName;
			Status = stepStatus;
		}
		public bool Equals(StepResult other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.StepNumber == StepNumber && other.TotalStepsCount == TotalStepsCount && Equals(other.Name, Name) && Equals(other.Status, Status);
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(StepResult)) return false;
			return Equals((StepResult)obj);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				int result = StepNumber;
				result = (result * 397) ^ TotalStepsCount;
				result = (result * 397) ^ (Name != null ? Name.GetHashCode() : 0);
				result = (result * 397) ^ Status.GetHashCode();
				return result;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}/{1} {2}: {3}", StepNumber, TotalStepsCount, Name, Status);
		}
	}
}