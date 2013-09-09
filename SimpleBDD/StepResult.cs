namespace SimpleBDD
{
	public class StepResult
	{
		public int StepNumber { get; private set; }
		public int TotalStepsCount { get; private set; }
		public string Name { get; private set; }
		public ResultStatus Status { get; private set; }

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
	}
}