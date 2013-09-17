using LightBDD.Coordination;
using LightBDD.Results.Implementation;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests.Coordination
{
	[TestFixture]
	public class FeatureCoordinatorTests
	{
		private IFeatureAggregator _aggregator;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_aggregator = MockRepository.GenerateMock<IFeatureAggregator>();
			FeatureCoordinator.Instance.Aggregator = _aggregator;
		}

		#endregion

		[Test]
		public void Should_add_feature()
		{
			var feature = new FeatureResult("name", "desc", "label");
			FeatureCoordinator.Instance.AddFeature(feature);
			_aggregator.AssertWasCalled(a => a.AddFeature(feature));
		}

		[Test]
		public void Should_finish()
		{
			FeatureCoordinator.Instance.Finished();
			_aggregator.AssertWasCalled(a => a.Finished());
		}
	}
}