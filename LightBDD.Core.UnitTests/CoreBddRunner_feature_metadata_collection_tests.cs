using LightBDD.Core.UnitTests.TestableIntegration;
using Xunit;

namespace LightBDD.Core.UnitTests
{
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

        [Fact]
        public void It_should_collect_all_feature_details()
        {
            var feature = new TestableBddRunner(typeof(Feature_with_all_details)).GetFeatureResult();
            Assert.Equal("Feature with all details", feature.Info.Name.ToString());
            Assert.Equal(new[] { "Ticket-1", "Ticket-2" }, feature.Info.Labels);
            Assert.Equal("Runner tests description", feature.Info.Description);
        }

        [Fact]
        public void It_should_collect_feature_with_labels()
        {
            var feature = new TestableBddRunner(typeof(Feature_with_labels)).GetFeatureResult();
            Assert.Equal("Feature with labels", feature.Info.Name.ToString());
            Assert.Equal(new[] { "Ticket-1", "Ticket-2" }, feature.Info.Labels);
            Assert.Null(feature.Info.Description);
        }

        [Fact]
        public void It_should_collect_feature_with_description()
        {
            var feature = new TestableBddRunner(typeof(Feature_with_description)).GetFeatureResult();
            Assert.Equal("Feature with description", feature.Info.Name.ToString());
            Assert.Equal("Runner tests description", feature.Info.Description);
            Assert.Empty(feature.Info.Labels);
        }

        [Fact]
        public void It_should_collect_plain_feature()
        {
            var feature = new TestableBddRunner(typeof(Feature_without_details)).GetFeatureResult();
            Assert.Equal("Feature without details", feature.Info.Name.ToString());
            Assert.Null(feature.Info.Description);
            Assert.Empty(feature.Info.Labels);
        }
    }
}