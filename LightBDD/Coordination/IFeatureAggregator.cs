using LightBDD.Results;

namespace LightBDD.Coordination
{
	/// <summary>
	/// Feature aggregator interface for collecting feature results.
	/// </summary>
	public interface IFeatureAggregator
	{
		/// <summary>
		/// Aggregates given feature.
		/// </summary>
		/// <param name="feature">Feature to aggregate.</param>
		void AddFeature(IFeatureResult feature);
		/// <summary>
		/// Notifies aggregator that no more features would be added.
		/// </summary>
		void Finished();
	}
}