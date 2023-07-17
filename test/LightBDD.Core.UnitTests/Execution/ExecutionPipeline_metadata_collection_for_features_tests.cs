using System.Threading.Tasks;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class ExecutionPipeline_metadata_collection_for_features_tests
{
    [FeatureDescription("Runner tests description")]
    [Label("Ticket-1")]
    [Label("Ticket-2")]
    class Feature_with_all_details
    {
        [TestScenario]
        public void Scenario1()
        {
        }
    }

    [Label("Ticket-1")]
    [Label("Ticket-2")]
    class Feature_with_labels
    {
        [TestScenario]
        public void Scenario1()
        {
        }
    }

    [FeatureDescription("Runner tests description")]
    class Feature_with_description
    {
        [TestScenario]
        public void Scenario1()
        {
        }
    }

    class Feature_without_details
    {
        [TestScenario]
        public void Scenario1()
        {
        }
    }

    [Test]
    public async Task It_should_collect_all_feature_details()
    {
        var feature = await TestableCoreExecutionPipeline.Default.ExecuteFeature(typeof(Feature_with_all_details));
        Assert.That(feature.Info.Name.ToString(), Is.EqualTo(nameof(Feature_with_all_details)));
        Assert.That(feature.Info.Labels, Is.EqualTo(new[] { "Ticket-1", "Ticket-2" }));
        Assert.That(feature.Info.Description, Is.EqualTo("Runner tests description"));
    }

    [Test]
    public async Task It_should_collect_feature_with_labels()
    {
        var feature = await TestableCoreExecutionPipeline.Default.ExecuteFeature(typeof(Feature_with_labels));
        Assert.That(feature.Info.Name.ToString(), Is.EqualTo(nameof(Feature_with_labels)));
        Assert.That(feature.Info.Labels, Is.EqualTo(new[] { "Ticket-1", "Ticket-2" }));
        Assert.Null(feature.Info.Description);
    }

    [Test]
    public async Task It_should_collect_feature_with_description()
    {
        var feature = await TestableCoreExecutionPipeline.Default.ExecuteFeature(typeof(Feature_with_description));
        Assert.That(feature.Info.Name.ToString(), Is.EqualTo(nameof(Feature_with_description)));
        Assert.That(feature.Info.Description, Is.EqualTo("Runner tests description"));
        Assert.That(feature.Info.Labels, Is.Empty);
    }

    [Test]
    public async Task It_should_collect_plain_feature()
    {
        var feature = await TestableCoreExecutionPipeline.Default.ExecuteFeature(typeof(Feature_without_details));
        Assert.That(feature.Info.Name.ToString(), Is.EqualTo(nameof(Feature_without_details)));
        Assert.Null(feature.Info.Description);
        Assert.That(feature.Info.Labels, Is.Empty);
    }
}