using System.Collections.Generic;

namespace LightBDD.Results
{
	/// <summary>
	/// Interface describing feature test result.
	/// </summary>
	public interface IFeatureResult
	{
		/// <summary>
		/// Feature description.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Feature name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Returns executed scenarios for given feature.
		/// </summary>
		IEnumerable<IScenarioResult> Scenarios { get; }

		/// <summary>
		/// [Label] attribute associated to feature.
		/// </summary>
		string Label { get; }
	}
}