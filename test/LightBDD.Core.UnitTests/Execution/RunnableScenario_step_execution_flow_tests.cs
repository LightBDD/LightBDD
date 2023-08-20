using System;
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
        Task MyScenario(ICoreScenarioStepsRunner runner) => runner.Test().TestScenario(
            Given_step_one,
            When_step_two,
            Then_step_three);

        var result = await TestableScenarioFactory.Default.RunScenario(MyScenario);

        StepResultExpectation.AssertEqual(result.GetSteps(),
            new(1, 3, nameof(Given_step_one), ExecutionStatus.Passed),
            new(2, 3, nameof(When_step_two), ExecutionStatus.Passed),
            new(3, 3, nameof(Then_step_three), ExecutionStatus.Passed));

        result.Status.ShouldBe(ExecutionStatus.Passed);
    }

    [Test]
    public async Task Runner_should_propagate_step_exception_and_stop_executing_further_steps()
    {
        Task MyScenario(ICoreScenarioStepsRunner runner) => runner.Test().TestScenario(
            Given_step_one,
            When_step_two_throwing_exception,
            Then_step_three);

        var result = await TestableScenarioFactory.Default.RunScenario(MyScenario);

        StepResultExpectation.AssertEqual(result.GetSteps(),
            new(1, 3, nameof(Given_step_one), ExecutionStatus.Passed),
            new(2, 3, nameof(When_step_two_throwing_exception), ExecutionStatus.Failed, $"Step 2 Failed: System.InvalidOperationException: {ExceptionReason}"),
            new(3, 3, nameof(Then_step_three), ExecutionStatus.NotRun));

        result.Status.ShouldBe(ExecutionStatus.Failed);
        result.ExecutionException.ShouldBeOfType<InvalidOperationException>()
            .Message.ShouldBe("exception reason");
        result.StatusDetails.ShouldBe("Step 2 Failed: System.InvalidOperationException: exception reason");
    }

    [Test]
    public async Task Runner_should_capture_bypassed_and_ignored_steps()
    {
        Task MyScenario(ICoreScenarioStepsRunner runner) => runner.Test().TestScenario(
            Given_step_one,
            When_step_two_is_bypassed,
            Then_step_three_should_be_ignored,
            Then_step_four);

        var result = await TestableScenarioFactory.Default.RunScenario(MyScenario);

        StepResultExpectation.AssertEqual(result.GetSteps(),
            new(1, 4, nameof(Given_step_one), ExecutionStatus.Passed),
            new(2, 4, nameof(When_step_two_is_bypassed), ExecutionStatus.Bypassed, $"Step 2 Bypassed: {BypassReason}"),
            new(3, 4, nameof(Then_step_three_should_be_ignored), ExecutionStatus.Ignored,$"Step 3 Ignored: {IgnoreReason}"),
            new(4, 4, nameof(Then_step_four), ExecutionStatus.NotRun));

        result.Status.ShouldBe(ExecutionStatus.Ignored);
        result.ExecutionException.ShouldBeOfType<Core.Execution.IgnoreException>()
            .Message.ShouldBe(IgnoreReason);
        result.StatusDetails.ShouldBe($"Step 2 Bypassed: {BypassReason}{Environment.NewLine}Step 3 Ignored: {IgnoreReason}");
    }
}