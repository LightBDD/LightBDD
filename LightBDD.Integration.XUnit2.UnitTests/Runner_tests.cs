using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
using Xunit;
using Xunit.Abstractions;

namespace LightBDD.Integration.XUnit2.UnitTests
{
    [ScenarioCategory("Category E")]
    public class Runner_tests : FeatureFixture
    {
        [Scenario]
        [Label(nameof(It_should_capture_scenario_name))]
        public void It_should_capture_scenario_name()
        {
            Runner.Basic().RunScenario(Some_step);
            var result = GetScenarioResult(nameof(It_should_capture_scenario_name));
            Assert.Equal("It should capture scenario name", result.Info.Name.ToString());
        }

        [Scenario]
        [ScenarioCategory("Category C")]
        [Label(nameof(It_should_capture_nunit_specific_attributes))]
        public void It_should_capture_nunit_specific_attributes()
        {
            Runner.Basic().RunScenario(Some_step);

            var scenario = GetScenarioResult(nameof(It_should_capture_nunit_specific_attributes));
            Assert.Equal(new[]
            {
                "Category C",
                "Category E"
            }, scenario.Info.Categories.ToArray());
        }

        [Scenario]
        [Label(nameof(It_should_capture_nunit_ignore_assertion))]
        public void It_should_capture_nunit_ignore_assertion()
        {
            try
            {
                Runner.Basic().RunScenario(Ignored_step);
            }
            catch
            {
            }
            var result = GetScenarioResult(nameof(It_should_capture_nunit_ignore_assertion));
            Assert.Equal(ExecutionStatus.Ignored, result.Status);
        }

        [Scenario]
        public void Runner_should_throw_meaningful_exception_if_scenario_is_not_run_from_test()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => Runner.Basic().RunScenario(Some_step)));
            Assert.Equal(
                "Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and [assembly:Debuggable(true, true)] attribute is present in test assembly.",
                ex.Result.Message);
        }

        private void Ignored_step()
        {
            StepExecution.Current.IgnoreScenario("reason");
        }

        private void Some_step()
        {
        }

        private IScenarioResult GetScenarioResult(string scenarioId)
        {
            return FeatureFactory.GetRunnerFor(GetType())
                .GetFeatureResult()
                .GetScenarios()
                .Single(s => s.Info.Labels.Contains(scenarioId));
        }

        public Runner_tests(ITestOutputHelper output) : base(output)
        {
        }
    }
}
