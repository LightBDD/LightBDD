using System.IO;
using LightBDD.Coordination;
using NUnit.Framework;

namespace LightBDD.ConfigurationTests
{
    [TestFixture]
    public class FeatureCoordinatorTests
    {
        [Test]
        public void Should_initialize_writters_from_app_config()
        {
            FeatureCoordinator.Instance.Aggregator.Finished();
            Assert.True(File.Exists("Reports/FeaturesSummary.txt"), "Reports/FeaturesSummary.txt");
            Assert.True(File.Exists("Reports/FeaturesSummary.html"), "Reports/FeaturesSummary.html");
            Assert.True(File.Exists("Reports/FeaturesSummary.xml"), "Reports/FeaturesSummary.xml");
        }
    }
}