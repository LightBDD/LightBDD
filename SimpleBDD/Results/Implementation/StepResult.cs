namespace SimpleBDD.Results.Implementation
{
	internal class StepResult : IStepResult
	{
		public StepResult(int stepNumber, string stepName, ResultStatus stepStatus)
		{
			Number = stepNumber;
			Name = stepName;
			Status = stepStatus;
		}

		#region IStepResult Members

		public string Name { get; set; }
		public ResultStatus Status { get; set; }
		public int Number { get; set; }

		#endregion

		public bool Equals(StepResult other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Number == Number && Equals(other.Name, Name) && Equals(other.Status, Status);
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
				int result = Number;
				result = (result * 397) ^ (Name != null ? Name.GetHashCode() : 0);
				result = (result * 397) ^ Status.GetHashCode();
				return result;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} {1}: {2}", Number, Name, Status);
		}
	}
}