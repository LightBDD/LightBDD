using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class RunnableScenario_execution_tests
    {
        [Test]
        public async Task Run_should_pass_scenario()
        {
            var result = await TestableScenarioFactory.Default.CreateScenario(_ => Task.CompletedTask)
                .RunAsync();

            result.Status.ShouldBe(ExecutionStatus.Passed);
            result.StatusDetails.ShouldBeNull();
            result.ExecutionException.ShouldBeNull();
            result.ExecutionTime.ShouldNotBe(ExecutionTime.None);
            result.GetSteps().ShouldBeEmpty();
        }

        [Test]
        public async Task Run_result_should_be_available_via_Result_property()
        {
            var scenario = TestableScenarioFactory.Default.CreateScenario(_ => Task.CompletedTask);
            var result = await scenario.RunAsync();
            result.ShouldBeSameAs(scenario.Result);
        }
    }
}
