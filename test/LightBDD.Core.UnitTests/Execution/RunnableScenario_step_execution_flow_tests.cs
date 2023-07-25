using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class RunnableScenario_step_execution_flow_tests : Steps
{
    [Test]
    public async Task It_should_run_provided_steps()
    {
        Task MyScenario(ICoreScenarioStepsRunner runner) => runner.Test().TestScenario(Given_step_one, When_step_two, Then_step_three);

        var result = await TestableScenarioFactory.Default.CreateScenario(MyScenario).RunAsync();

        result.Status.ShouldBe(ExecutionStatus.Passed);
        StepResultExpectation.AssertEqual(result.GetSteps(),
            new(1, 3, nameof(Given_step_one), ExecutionStatus.Passed),
            new(2, 3, nameof(When_step_two), ExecutionStatus.Passed),
            new(3, 3, nameof(Then_step_three), ExecutionStatus.Passed));
    }
}