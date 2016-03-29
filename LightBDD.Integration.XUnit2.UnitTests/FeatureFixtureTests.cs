using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;
using Xunit;
using Xunit.Sdk;

namespace LightBDD.Integration.XUnit2.UnitTests
{
    public class FeatureFixtureTests
    {
        class TestableFeatureFixture : FeatureFixture
        {
            public IBddRunner GetRunner() => Runner;

            public TestableFeatureFixture(IProgressNotifier notifier) : base(new TestOutputHelper(), () => notifier) { }
        }

        [Fact]
        public void Progress_notifier_should_be_customizable()
        {
            var expected = new NoProgressNotifier();
            var actual = new TestableFeatureFixture(expected).GetRunner().Integrate().IntegrationContext.ProgressNotifier;
            Assert.Same(expected, actual);
        }
    }
}