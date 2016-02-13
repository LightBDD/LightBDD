using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Core.UnitTests.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [CustomFeatureDescription("some description")]
    [CustomCategory("some global category")]
    [ScenarioCategory("standard global category")]
    [TestFixture]
    public class CoreBddRunner_extensibility_tests : Steps
    {
        [Test]
        [CustomCategory("some local category")]
        [ScenarioCategory("standard local category")]
        public void It_should_collect_custom_categories()
        {
            IBddRunner runner = new TestableBddRunner(GetType());
            runner.TestScenario(Some_step);
            var scenario = runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Info.Categories, Is.EqualTo(new[] { "some global category", "some local category", "standard global category", "standard local category" }));
        }

        [Test]
        public void It_should_collect_custom_feature_description()
        {
            IBddRunner runner = new TestableBddRunner(GetType());
            runner.TestScenario(Some_step);
            Assert.That(runner.Integrate().GetFeatureResult().Info.Description, Is.EqualTo("some description"));
        }

        [CustomFeatureDescription("custom description")]
        [FeatureDescription("standard description")]
        class Feature_with_two_descriptions
        {
        }

        [Test]
        public void It_should_collect_standard_feature_description_if_both_are_specified()
        {
            IBddRunner runner = new TestableBddRunner(typeof(Feature_with_two_descriptions));
            runner.TestScenario(Some_step);
            Assert.That(runner.Integrate().GetFeatureResult().Info.Description, Is.EqualTo("standard description"));
        }

        [Test]
        public void It_should_capture_step_status_with_custom_exception_mapping()
        {
            IBddRunner runner = new TestableBddRunner(GetType());
            Assert.Throws<CustomIgnoreException>(() => runner.TestScenario(
                Given_step_one,
                When_step_two_ignoring_scenario,
                Then_step_three));

            var steps = runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, "Given step one", ExecutionStatus.Passed),
                new StepResultExpectation(2, "When step two ignoring scenario", ExecutionStatus.Ignored, IgnoreReason),
                new StepResultExpectation(3, "Then step three", ExecutionStatus.NotRun));
        }
    }
}