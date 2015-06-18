using LightBDD.Coordination;
using LightBDD.Results;
using Rhino.Mocks;
using Xunit;

namespace LightBDD.XUnit.UnitTests
{
    public class FeatureFixtureTests
    {
        class TestableFixture : FeatureFixture
        {
            public new BDDRunner Runner { get { return base.Runner; } }
        }

        class TestableFixture2 : FeatureFixture
        {
            public new BDDRunner Runner { get { return base.Runner; } }
        }

        [Fact]
        public void Two_instances_of_same_fixture_should_share_the_same_runner()
        {
            var i1 = new TestableFixture();
            var i2 = new TestableFixture();
            var i3 = new TestableFixture2();
            var i4 = new TestableFixture2();

            Assert.Same(i1.Runner, i2.Runner);
            Assert.Same(i3.Runner, i4.Runner);
            Assert.NotSame(i1.Runner, i3.Runner);
        }

        [Fact]
        public void Ony_one_result_per_test_class_should_be_returned()
        {
            var original = FeatureCoordinator.Instance.Aggregator;
            try
            {
                var aggregator = MockRepository.GenerateMock<IFeatureAggregator>();
                FeatureCoordinator.Instance.Aggregator = aggregator;

                new TestableFixture();
                new TestableFixture();
                new TestableFixture2();
                new TestableFixture2();

                FeatureCoordinator.Instance.Finished();
                aggregator.AssertWasCalled(a => a.AddFeature(Arg<IFeatureResult>.Is.Anything), o => o.Repeat.Twice());
            }
            finally
            {
                FeatureCoordinator.Instance.Aggregator = original;
            }
        }
    }
}