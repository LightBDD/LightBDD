using LightBDD.Notification;
using NUnit.Framework;

namespace LightBDD.NUnit3.UnitTests
{
    [TestFixture]
    public class FeatureFixtureTests
    {
        class TestFeatureFixture : FeatureFixture
        {
            public new IProgressNotifier CreateProgressNotifier()
            {
                return base.CreateProgressNotifier();
            }
        }

        [Test]
        public void Fixture_should_use_SimplifiedConsoleProgressNotifier_and_XUnitOutputProgressNotifier_notifiers()
        {
            var progressNotifier = new TestFeatureFixture().CreateProgressNotifier();
            Assert.That(progressNotifier,Is.InstanceOf<NUnit3OutputProgressNotifier>());
        }
    }
}