using System.Linq;
using LightBDD.Coordination;
using LightBDD.Notification;
using LightBDD.Results;
using Rhino.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace LightBDD.XUnit2.UnitTests
{
    public class FeatureFixtureTests
    {
        class TestableFixture : FeatureFixture
        {
            public TestableFixture()
                : base(MockRepository.GenerateMock<ITestOutputHelper>())
            {
            }
            public new BDDRunner Runner { get { return base.Runner; } }

            public new IProgressNotifier CreateProgressNotifier()
            {
                return base.CreateProgressNotifier();
            }
        }

        class TestableFixture2 : FeatureFixture
        {
            public TestableFixture2()
                : base(MockRepository.GenerateMock<ITestOutputHelper>())
            {
            }
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

        [Fact]
        public void Fixture_should_use_SimplifiedConsoleProgressNotifier_and_XUnitOutputProgressNotifier_notifiers()
        {
            var progressNotifier = new TestableFixture().CreateProgressNotifier();
            var delegatingNotifier = Assert.IsType<DelegatingProgressNotifier>(progressNotifier);
            Assert.True(delegatingNotifier.Notifiers.Any(n => n is SimplifiedConsoleProgressNotifier), "Delegating notifier does not use SimplifiedConsoleProgressNotifier");
            Assert.True(delegatingNotifier.Notifiers.Any(n => n is XUnitOutputProgressNotifier), "Delegating notifier does not use XUnitOutputProgressNotifier");
        }
    }
}