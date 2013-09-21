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
		string Name { get; }

		/// <summary>
		/// Step number.
		/// </summary>
		int Number { get; }

		/// <summary>
		/// Step status.
		/// </summary>
		ResultStatus Status { get; }

		/// <summary>
		/// Status details.
		/// It is useful for ignored or failed tests.
		/// It may be null if no additional details are provided.
		/// </summary>
		string StatusDetails { get; }
	}
}