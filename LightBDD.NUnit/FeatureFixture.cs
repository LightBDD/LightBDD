using LightBDD.Coordination;
using LightBDD.Notification;
using NUnit.Framework;

namespace LightBDD
{
	/// <summary>
	/// Base class for feature tests.
	/// It creates new BDDRunner instance on FixtureSetUp() and collects runner results and passes them to FeatureCoordinator on FixtureTearDown().
	/// </summary>
	public abstract class FeatureFixture
	{
		/// <summary>
		/// BDD Runner that should be used to run feature tests.
		/// </summary>
		protected BDDRunner Runner { get; private set; }

		/// <summary>
		/// Creates new BDDRunner instance.
		/// </summary>
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			Runner = new BDDRunner(GetType(), CreateProgressNotifier());
		}

		/// <summary>
		/// Collects runner results and passes them to FeatureCoordinator.
		/// </summary>
		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			FeatureCoordinator.Instance.AddFeature(Runner.Result);
		}

		/// <summary>
		/// Creates progress notifier used later by BDDRunner.
		/// Default implementation returns ConsoleProgressNotifier.
		/// </summary>
		/// <returns>progress notifier.</returns>
		protected virtual IProgressNotifier CreateProgressNotifier() { return new ConsoleProgressNotifier(); }
	}
}
