using System.Linq;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.Runner.IntegrationTests.Helpers;
using Shouldly;
using Xunit;

namespace LightBDD.Runner.IntegrationTests
{
    public class IntegrationTests
    {
        [Fact]
        public void It_should_execute_all_scenarios()
        {
            ProgressCapture.TestRunResult.OverallStatus.ShouldBe(ExecutionStatus.Passed);
        }

        [Fact]
        public void It_should_cover_passing_scenario()
        {
            ProgressCapture.GetScenarioResult(nameof(Integration_scenarios.Passing_scenario)).Status.ShouldBe(ExecutionStatus.Passed);
        }

        [Fact]
        public void It_should_cover_ignored_scenario()
        {
            ProgressCapture.GetScenarioResult(nameof(Integration_scenarios.Ignored_scenario)).Status.ShouldBe(ExecutionStatus.Ignored);
        }

        [Fact]
        public void It_should_cover_parameterized_scenario()
        {
            ProgressCapture.GetScenarioResults(nameof(Integration_scenarios.Parameterized_scenario))
                .Select(r => r.Info.Name.ToString())
                .ShouldBe(new[]
                {
                    "Parameterized scenario \"1\"",
                    "Parameterized scenario \"2\""
                });
        }

        public class Integration_scenarios : FeatureFixture
        {
            [Scenario]
            [Label(nameof(Passing_scenario))]
            public void Passing_scenario() => Runner.RunScenario(Some_step);

            [Scenario]
            [IgnoreScenario("ignore reason")]
            [Label(nameof(Ignored_scenario))]
            public void Ignored_scenario() => Runner.RunScenario(Some_step);

            [Scenario]
            [Label(nameof(Ignored_scenario_imperative_way))]
            public void Ignored_scenario_imperative_way() => StepExecution.Current.Ignore("ignore reason");

            [Scenario]
            [ScenarioInlineCase(1)]
            [ScenarioInlineCase(2)]//TODO: add option to skip case
            [Label(nameof(Parameterized_scenario))]
            public void Parameterized_scenario(int scenario) => Runner.RunScenario(Some_step);

            private void Some_step() { }
        }
    }
}
