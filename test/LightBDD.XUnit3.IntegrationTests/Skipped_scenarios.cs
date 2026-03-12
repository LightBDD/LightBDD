using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;
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
                "Scenario 2 [param: \"a\"]:Scenario: parameterized 1",
                "Scenario 2 [param: \"b\"]:Scenario: parameterized 2",
                "Scenario 3 [param: \"val1\"]:Scenario: skip all",
                "Scenario 3 [param: \"val2\"]:Scenario: skip all",
                "Scenario 4 [param: \"skip_me\"]:Scenario: mixed skip"
            };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Non_skipped_rows_should_pass_when_mixed_with_skipped_rows()
        {
            var result = ScenarioProgressCapture.Instance.Results
                .SingleOrDefault(x => x.Info.Name.ToString() == "Scenario 4 [param: \"run_me\"]");

            Assert.NotNull(result);
            Assert.Equal(ExecutionStatus.Passed, result.Status);
        }

        [Scenario(Skip = "normal scenario")]
        public async Task Scenario_1()
        {
            await Runner.RunScenarioAsync(
                _ => Given_something(),
                _ => When_something_happens(),
                _ => Then_something_else_should_happen());
        }

        [Scenario]
        [InlineData("a", Skip = "parameterized 1")]
        [InlineData("b", Skip = "parameterized 2")]
        public async Task Scenario_2(string param)
        {
            await Runner.RunScenarioAsync(
                _ => Given_something(),
                _ => When_something_happens(),
                _ => Then_something_else_should_happen());
        }

        [Scenario(Skip = "skip all")]
        [InlineData("val1")]
        [InlineData("val2")]
        public async Task Scenario_3(string param)
        {
            await Runner.RunScenarioAsync(
                _ => Given_something(),
                _ => When_something_happens(),
                _ => Then_something_else_should_happen());
        }

        [Scenario]
        [InlineData("run_me")]
        [InlineData("skip_me", Skip = "mixed skip")]
        public async Task Scenario_4(string param)
        {
            await Runner.RunScenarioAsync(
                _ => Given_a_passing_step());
        }

        private Task Given_a_passing_step()
        {
            return Task.CompletedTask;
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

    }
}
