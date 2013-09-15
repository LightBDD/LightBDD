using System.Collections.Generic;

namespace SimpleBDD.Results
{
	/// <summary>
	/// Interface describing scenario test result.
	/// </summary>
	public interface IScenarioResult
	{
		/// <summary>
		/// Scenario name.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Scenario status.
		/// </summary>
		ResultStatus Status { get; }
		/// <summary>
		/// Scenario steps.
		/// </summary>
		IEnumerable<IStepResult> Steps { get; }
	}
}