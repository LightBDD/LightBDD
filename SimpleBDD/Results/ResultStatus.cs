using System;

namespace SimpleBDD.Results
{
	/// <summary>
	/// Represents status of test / scenario.
	/// </summary>
	[Serializable]
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
		/// Ignored / skipped
		/// </summary>
		Ignored,
		/// <summary>
		/// Failed
		/// </summary>
		Failed
	}
}