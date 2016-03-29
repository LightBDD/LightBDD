using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Integration.MsTest.UnitTests
{
    [TestClass]
    public class FeatureFixtureTests
    {
        class TestableFeatureFixture : FeatureFixture
        {
            public IBddRunner GetRunner() => Runner;

            public TestableFeatureFixture(IProgressNotifier notifier) : base(() => notifier) { }
        }

        [TestMethod]
        public void Progress_notifier_should_be_customizable()
        {
            var expected = new NoProgressNotifier();
            var actual = new TestableFeatureFixture(expected).GetRunner().Integrate().IntegrationContext.ProgressNotifier;
            Assert.AreSame(expected, actual);
        }
    }
}