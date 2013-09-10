namespace SimpleBDD.Results
{
	/// <summary>
	/// Represents status of test / scenario.
	/// </summary>
	public enum ResultStatus
	{
		/// <summary>
		/// Not run yet
		/// </summary>
		NotRun,
		/// <summary>
		/// Passed
		/// </summary>
		Passed,
		/// <summary>
		/// Failed
		/// </summary>
		Failed,
		/// <summary>
		/// Ignored / skipped
		/// </summary>
		Ignored
	}
}