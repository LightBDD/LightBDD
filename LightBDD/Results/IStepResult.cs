namespace LightBDD.Results
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
		/// Step number.
		/// </summary>
		int Number { get; set; }

		/// <summary>
		/// Step status.
		/// </summary>
		ResultStatus Status { get; set; }
	}
}