using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2.IntegrationTests.Helpers;
using Xunit;

namespace LightBDD.XUnit2.IntegrationTests
{
    [TestCaseOrderer("LightBDD.XUnit2.IntegrationTests." + nameof(RunScenariosFirst), "LightBDD.XUnit2.IntegrationTests")]
    public class Skipped_scenarios : FeatureFixture

    {
        [Fact]
        public void All_skipped_tests_should_be_included_in_results()
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
                "Scenario 3 [param: \"val2\"]:Scenario: skip all"
            };
            Assert.Equal(expected, actual);
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