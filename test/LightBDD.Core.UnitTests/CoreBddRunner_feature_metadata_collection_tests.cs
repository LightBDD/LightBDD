using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_feature_metadata_collection_tests
    {
        [FeatureDescription("Runner tests description")]
        [Label("Ticket-1")]
        [Label("Ticket-2")]
        class Feature_with_all_details { }

        [Label("Ticket-1")]
        [Label("Ticket-2")]
        class Feature_with_labels { }

        [FeatureDescription("Runner tests description")]
        class Feature_with_description { }

        class Feature_without_details { }

        [Test]
        public void It_should_collect_all_feature_details()
        {
            var feature = TestableFeatureRunnerRepository.GetRunner(typeof(Feature_with_all_details)).GetFeatureResult();
            Assert.That(feature.Info.Name.ToString(), Is.EqualTo("Feature with all details"));
            Assert.That(feature.Info.Labels, Is.EqualTo(new[] { "Ticket-1", "Ticket-2" }));
            Assert.That(feature.Info.Description, Is.EqualTo("Runner tests description"));
        }

        [Test]
        public void It_should_collect_feature_with_labels()
        {
            var feature = TestableFeatureRunnerRepository.GetRunner(typeof(Feature_with_labels)).GetFeatureResult();
            Assert.That(feature.Info.Name.ToString(), Is.EqualTo("Feature with labels"));
            Assert.That(feature.Info.Labels, Is.EqualTo(new[] { "Ticket-1", "Ticket-2" }));
            Assert.Null(feature.Info.Description);
        }

        [Test]
        public void It_should_collect_feature_with_description()
        {
            var feature = TestableFeatureRunnerRepository.GetRunner(typeof(Feature_with_description)).GetFeatureResult();
            Assert.That(feature.Info.Name.ToString(), Is.EqualTo("Feature with description"));
            Assert.That(feature.Info.Description, Is.EqualTo("Runner tests description"));
            Assert.That(feature.Info.Labels, Is.Empty);
        }

        [Test]
        public void It_should_collect_plain_feature()
        {
            var feature = TestableFeatureRunnerRepository.GetRunner(typeof(Feature_without_details)).GetFeatureResult();
            Assert.That(feature.Info.Name.ToString(), Is.EqualTo("Feature without details"));
            Assert.Null(feature.Info.Description);
            Assert.That(feature.Info.Labels, Is.Empty);
        }
    }
}