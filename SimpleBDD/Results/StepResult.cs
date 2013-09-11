namespace SimpleBDD.Results
{
	/// <summary>
	/// Step result containing step name, its number in a list and its status.
	/// </summary>
	public class StepResult
	{
		/// <summary>
		/// Step number.
		/// </summary>
		public int StepNumber { get; private set; }
		/// <summary>
		/// Total number of steps.
		/// </summary>
		public int TotalStepsCount { get; private set; }
		/// <summary>
		/// Step name.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Step status.
		/// </summary>
		public ResultStatus Status { get; set; }

		/// <summary>
		/// Initializes step result with all data.
		/// </summary>
		/// <param name="stepNumber">Step number.</param>
		/// <param name="totalStepsCount">Total number of steps.</param>
		/// <param name="stepName">Step name.</param>
		/// <param name="stepStatus">Step status.</param>
		public StepResult(int stepNumber, int totalStepsCount, string stepName, ResultStatus stepStatus)
		{
			StepNumber = stepNumber;
			TotalStepsCount = totalStepsCount;
			Name = stepName;
			Status = stepStatus;
		}

		/// <summary>
		/// Compares two instances of step results.
		/// </summary>
		/// <param name="other">Other step.</param>
		/// <returns>True if equal, otherwise false.</returns>
		public bool Equals(StepResult other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.StepNumber == StepNumber && other.TotalStepsCount == TotalStepsCount && Equals(other.Name, Name) && Equals(other.Status, Status);
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(StepResult)) return false;
			return Equals((StepResult)obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
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