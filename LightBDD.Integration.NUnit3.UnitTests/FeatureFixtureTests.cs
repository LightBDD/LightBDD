using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3.UnitTests
{
    [TestFixture]
    public class FeatureFixtureTests
    {
        class TestableFeatureFixture : FeatureFixture
        {
            public IBddRunner GetRunner() => Runner;

            public TestableFeatureFixture(IProgressNotifier notifier) : base(() => notifier) { }
        }

        [Test]
        public void Progress_notifier_should_be_customizable()
        {
            var expected = new NoProgressNotifier();
            var actual = new TestableFeatureFixture(expected).GetRunner().Integrate().IntegrationContext.ProgressNotifier;
            Assert.AreSame(expected, actual);
        }
    }
}