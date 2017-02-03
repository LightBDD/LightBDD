using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Framework.Scenarios.Basic;
using Xunit;
using Xunit.Abstractions;

namespace LightBDD.Integration.XUnit2.UnitTests
{
    [FeatureDescription("desc")]
    [ScenarioCategory("Category B")]
    public class IntegrationTests : FeatureFixture
    {
        public IntegrationTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Scenario]
        [Label(nameof(It_should_capture_scenario_name))]
        public void It_should_capture_scenario_name()
        {
            Runner.RunScenario(Some_step);
            var result = GetScenarioResult(nameof(It_should_capture_scenario_name));
            Assert.Equal("It should capture scenario name", result.Info.Name.ToString());
        }

        [Scenario]
        [Label(nameof(It_should_capture_scenario_name_after_await))]
        public async Task It_should_capture_scenario_name_after_await()
        {
            await Task.Yield();
            Runner.RunScenario(Some_step);
            var result = GetScenarioResult(nameof(It_should_capture_scenario_name_after_await));
            Assert.Equal("It should capture scenario name after await", result.Info.Name.ToString());
        }

        [Scenario]
        [ScenarioCategory("Category A")]
        [Label(nameof(It_should_capture_nunit_specific_attributes))]
        public void It_should_capture_nunit_specific_attributes()
        {
            Runner.RunScenario(Some_step);

            var result = XUnit2FeatureRunnerFactory.GetRunnerFor(GetType()).GetFeatureResult();
            Assert.Equal("desc", result.Info.Description);

            var scenario = GetScenarioResult(nameof(It_should_capture_nunit_specific_attributes));
            Assert.Equal(
                new[] { "Category A", "Category B" },
                scenario.Info.Categories.ToArray());
        }

        [Scenario]
        [Label(nameof(It_should_capture_nunit_ignore_assertion))]
        public void It_should_capture_nunit_ignore_assertion()
        {
            try
            {
                Runner.RunScenario(Ignored_step);
            }
            catch
            {
            }
            var result = GetScenarioResult(nameof(It_should_capture_nunit_ignore_assertion));
            Assert.Equal(ExecutionStatus.Ignored, result.Status);
            Assert.Equal("Step 1: manually ignored", result.StatusDetails);
        }

        [Fact]
        public void Runner_should_throw_meaningful_exception_if_scenario_is_not_run_from_Scenario_attribute()
        {
            Exception ex = Assert.Throws<InvalidOperationException>(() => Runner.RunScenario(Some_step));
            Assert.Equal(
                "Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and test class deriving from FeatureFixture or with [FeatureFixture] attribute.",
                ex.Message);
        }

        private void Ignored_step()
        {
            StepExecution.Current.IgnoreScenario("manually ignored");
        }

        private void Some_step()
        {
        }

        private IScenarioResult GetScenarioResult(string scenarioId)
        {
            return XUnit2FeatureRunnerFactory.GetRunnerFor(GetType())
                .GetFeatureResult()
                .GetScenarios()
                .Single(s => s.Info.Labels.Contains(scenarioId));
        }
    }
}
