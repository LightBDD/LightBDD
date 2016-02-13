using System;
using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Core.UnitTests.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_scenario_metadata_collection_tests : Steps
    {
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _runner = new TestableBddRunner(GetType());
        }

        [Test]
        public void It_should_capture_scenario_name()
        {
            _runner.TestScenario(Some_step);
            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Info.Name.ToString(), Is.EqualTo("It should capture scenario name"));
            Assert.That(scenario.Info.Labels, Is.Empty);
            Assert.That(scenario.Info.Categories, Is.Empty);
        }

        [Test]
        [Label("Ticket-1")]
        [Label("Ticket-2")]
        public void It_should_capture_scenario_name_with_labels()
        {
            _runner.TestScenario(Some_step);
            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Info.Name.ToString(), Is.EqualTo("It should capture scenario name with labels"));
            Assert.That(scenario.Info.Labels, Is.EqualTo(new[] { "Ticket-1", "Ticket-2" }));
            Assert.That(scenario.Info.Categories, Is.Empty);
        }

        [Test]
        [Label("Ticket-1")]
        [Label("Ticket-2")]
        [ScenarioCategory("catA")]
        [ScenarioCategory("catB")]
        public void It_should_capture_scenario_name_with_categories()
        {
            _runner.TestScenario(Some_step);
            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Info.Name.ToString(), Is.EqualTo("It should capture scenario name with categories"));
            Assert.That(scenario.Info.Labels, Is.EqualTo(new[] { "Ticket-1", "Ticket-2" }));
            Assert.That(scenario.Info.Categories, Is.EqualTo(new[] { "catA", "catB" }));
        }

        [Test]
        public void It_should_capture_scenario_execution_status_for_passing_steps()
        {
            _runner.TestScenario(
                Given_step_one,
                When_step_two,
                Then_step_three);

            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Passed));
            Assert.Null(scenario.StatusDetails);
        }

        [Test]
        public void It_should_capture_scenario_execution_status_for_failing_steps()
        {
            try
            {
                _runner.TestScenario(
                    Given_step_one,
                    When_step_two_throwing_exception,
                    Then_step_three);
            }
            catch { }

            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Step 2: " + ExceptionReason));
        }

        [Test]
        public void It_should_capture_scenario_status_for_passing_steps_with_bypassed_one()
        {
            _runner.TestScenario(
                Given_step_one,
                When_step_two_is_bypassed,
                Then_step_three);

            Assert.That(_runner.Integrate().GetFeatureResult().GetScenarios().Single().Status, Is.EqualTo(ExecutionStatus.Bypassed));
        }

        [Test]
        public void It_should_capture_scenario_status_for_ignored_steps()
        {
            try
            {
                _runner.TestScenario(
                Given_step_one,
                When_step_two_is_bypassed,
                Then_step_three_should_be_ignored,
                Then_step_four);
            }
            catch { }

            Assert.That(_runner.Integrate().GetFeatureResult().GetScenarios().Single().Status, Is.EqualTo(ExecutionStatus.Ignored));
        }

        [Test]
        public void It_should_capture_scenario_execution_status_details_from_all_steps()
        {
            try
            {
                _runner.TestScenario(
                    Given_step_one,
                    When_step_two_is_bypassed,
                    Then_step_three_should_throw_exception);
            }
            catch { }

            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            var expected = "Step 2: " + BypassReason + Environment.NewLine +
                           "Step 3: " + ExceptionReason;

            Assert.That(scenario.StatusDetails, Is.EqualTo(expected));
        }
    }
}