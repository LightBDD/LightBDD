using System;
using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Core.UnitTests.TestableIntegration;
using Xunit;

namespace LightBDD.Core.UnitTests
{
    public class CoreBddRunner_scenario_metadata_collection_tests : Steps
    {
        private readonly IBddRunner _runner;

        public CoreBddRunner_scenario_metadata_collection_tests()
        {
            _runner = new TestableBddRunner(GetType());
        }

        [Fact]
        public void It_should_capture_scenario_name()
        {
            _runner.TestScenario(Some_step);
            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.Equal("It should capture scenario name", scenario.Info.Name.ToString());
            Assert.Empty(scenario.Info.Labels);
            Assert.Empty(scenario.Info.Categories);
        }

        [Fact]
        [Label("Ticket-1")]
        [Label("Ticket-2")]
        public void It_should_capture_scenario_name_with_labels()
        {
            _runner.TestScenario(Some_step);
            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.Equal("It should capture scenario name with labels", scenario.Info.Name.ToString());
            Assert.Equal(new[] { "Ticket-1", "Ticket-2" }, scenario.Info.Labels);
            Assert.Empty(scenario.Info.Categories);
        }

        [Fact]
        [Label("Ticket-1")]
        [Label("Ticket-2")]
        [ScenarioCategory("catA")]
        [ScenarioCategory("catB")]
        public void It_should_capture_scenario_name_with_categories()
        {
            _runner.TestScenario(Some_step);
            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.Equal("It should capture scenario name with categories", scenario.Info.Name.ToString());
            Assert.Equal(new[] { "Ticket-1", "Ticket-2" }, scenario.Info.Labels);
            Assert.Equal(new[] { "catA", "catB" }, scenario.Info.Categories);
        }

        [Fact]
        public void It_should_capture_scenario_execution_status_for_passing_steps()
        {
            _runner.TestScenario(
                Given_step_one,
                When_step_two,
                Then_step_three);

            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.Equal(ExecutionStatus.Passed, scenario.Status);
            Assert.Null(scenario.StatusDetails);
        }

        [Fact]
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
            Assert.Equal(ExecutionStatus.Failed, scenario.Status);
            Assert.Equal("Step 2: " + ExceptionReason, scenario.StatusDetails);
        }

        [Fact]
        public void It_should_capture_scenario_status_for_passing_steps_with_bypassed_one()
        {
            _runner.TestScenario(
                Given_step_one,
                When_step_two_is_bypassed,
                Then_step_three);

            Assert.Equal(ExecutionStatus.Bypassed, _runner.Integrate().GetFeatureResult().GetScenarios().Single().Status);
        }

        [Fact]
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

            Assert.Equal(ExecutionStatus.Ignored, _runner.Integrate().GetFeatureResult().GetScenarios().Single().Status);
        }

        [Fact]
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
            Assert.Equal(ExecutionStatus.Failed, scenario.Status);
            var expected = "Step 2: " + BypassReason + Environment.NewLine +
                           "Step 3: " + ExceptionReason;

            Assert.Equal(expected, scenario.StatusDetails);
        }
    }
}