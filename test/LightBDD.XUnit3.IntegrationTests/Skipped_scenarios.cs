using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit3.IntegrationTests.Helpers;
using Xunit;

namespace LightBDD.XUnit3.IntegrationTests
{
    [TestCaseOrderer(typeof(RunScenariosFirst))]
    public class Skipped_scenarios : FeatureFixture
    {
        [Fact]
        public void All_ignored_tests_should_be_included_in_results()
        {
            var actual = ScenarioProgressCapture.Instance.Results
                .Where(x => x.Status == ExecutionStatus.Ignored)
                .Select(x => $"{x.Info.Name}:{x.StatusDetails}")
                .OrderBy(x => x)
                .ToArray();

            var expected = new[]
            {
                "Scenario 1:Scenario: normal scenario",
                "Scenario 2 [param: \"a\"]:Step 1: parameterized 1",
                "Scenario 2 [param: \"b\"]:Step 1: parameterized 2",
                "Scenario 3 [param: \"val1\"]:Scenario: ignore all",
                "Scenario 3 [param: \"val2\"]:Scenario: ignore all"
            };
            Assert.Equal(expected, actual);
        }

        [Scenario]
        [IgnoreScenario("normal scenario")]
        public async Task Scenario_1()
        {
            await Runner.RunScenarioAsync(
                _ => Given_something(),
                _ => When_something_happens(),
                _ => Then_something_else_should_happen());
        }

        [Scenario]
        [InlineData("a")]
        [InlineData("b")]
        public async Task Scenario_2(string param)
        {
            await Runner.RunScenarioAsync(
                _ => Ignore_with_reason_async($"parameterized {(param == "a" ? "1" : "2")}"),
                _ => Then_something_else_should_happen());
        }

        [Scenario]
        [IgnoreScenario("ignore all")]
        [InlineData("val1")]
        [InlineData("val2")]
        public async Task Scenario_3(string param)
        {
            await Runner.RunScenarioAsync(
                _ => Given_something(),
                _ => When_something_happens(),
                _ => Then_something_else_should_happen());
        }

        private Task Given_something()
        {
            throw new NotImplementedException();
        }

        private Task When_something_happens()
        {
            throw new NotImplementedException();
        }

        private Task Then_something_else_should_happen()
        {
            throw new NotImplementedException();
        }

        private void Ignore_with_reason(string reason)
        {
            StepExecution.Current.IgnoreScenario(reason);
        }

        private Task Ignore_with_reason_async(string reason)
        {
            StepExecution.Current.IgnoreScenario(reason);
            return Task.CompletedTask;
        }
    }
}
