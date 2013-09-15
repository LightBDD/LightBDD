namespace SimpleBDD.Results
{
	/// <summary>
	/// Interface describing scenario step test result.
	/// </summary>
	public interface IStepResult
	{
		/// <summary>
		/// Step name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Step status.
		/// </summary>
		ResultStatus Status { get; set; }

		/// <summary>
		/// Step number.
		/// </summary>
		int StepNumber { get; set; }

		/// <summary>
		/// Total number of steps.
		/// </summary>
		int TotalStepsCount { get; set; }
	}
}