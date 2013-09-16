using System.Runtime.ConstrainedExecution;
using LightBDD.Results;

namespace LightBDD.Coordination
{
	/// <summary>
	/// Feature coordinator singleton class allowing to collect feature results, pass them to specified aggregator and notify it when tests finish.
	/// It allows to customize aggregator - by default it is FeatureSummaryAggregator that saves feature results to XML.
	/// This class guarantees aggregator notification on AppDomain unload, but offers also method to notify aggregator manually.
	/// </summary>
	public class FeatureCoordinator : CriticalFinalizerObject
	{
		private static readonly FeatureCoordinator _instance = new FeatureCoordinator();

		private FeatureCoordinator()
		{
			Aggregator = new FeatureSummaryAggregator();
		}

		/// <summary>
		/// Aggregator used to collect feature results.
		/// It can be customized - the best time to set it is before any tests run (like in class with [SetUpFixture] attribute).
		/// By default, FeatureSummaryAggregator is used.
		/// </summary>
		public IFeatureAggregator Aggregator { get; set; }

		/// <summary>
		/// Coordinator instance.
		/// </summary>
		public static FeatureCoordinator Instance { get { return _instance; } }

		/// <summary>
		/// Adds feature to aggregator.
		/// </summary>
		/// <param name="feature">Feature to aggregate.</param>
		public void AddFeature(IFeatureResult feature)
		{
			Aggregator.AddFeature(feature);
		}

		/// <summary>
		/// Notifies aggregator that all features has been already added.
		/// PLEASE NOTE that this method does not have to be normally called - FeatureCoordinator will notify aggregator on AppDomain unload anyway.
		/// </summary>
		public void Finished()
		{
			Aggregator.Finished();
		}

		/// <summary>
		/// Notifies aggregator that all features has been already added.
		/// </summary>
		~FeatureCoordinator()
		{
			try
			{
				Finished();
			}
			catch
			{
			}
		}
	}
}
