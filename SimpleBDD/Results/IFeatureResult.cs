using System.Collections.Generic;

namespace SimpleBDD.Results
{
	/// <summary>
	/// Interface describing feature test result.
	/// </summary>
	public interface IFeatureResult
	{
		/// <summary>
		/// Returns executed scenarios for given feature.
		/// </summary>
		IEnumerable<IScenarioResult> Scenarios { get; } 
	}
}