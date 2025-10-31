using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using TUnit.Core.Exceptions;

namespace LightBDD.TUnit.UnitTests;

[System.ComponentModel.Description("desc")]
[Category("Category D")]
[ScenarioCategory("Category E")]
public class IntegrationTests : FeatureFixture
{
    [Scenario]
    [Label(nameof(It_should_capture_scenario_name))]
    public async Task It_should_capture_scenario_name()
    {
        Runner.RunScenario(Some_step);
        var result = GetScenarioResult(nameof(It_should_capture_scenario_name));
        await Assert.That(result.Info.Name.ToString()).IsEqualTo("It should capture scenario name");
    }

    [Scenario]
    [Label(nameof(It_should_capture_scenario_name_after_await))]
    public async Task It_should_capture_scenario_name_after_await()
    {
        await Task.Yield();
        Runner.RunScenario(Some_step);
        var result = GetScenarioResult(nameof(It_should_capture_scenario_name_after_await));
        await Assert.That(result.Info.Name.ToString()).IsEqualTo("It should capture scenario name after await");
    }

    [Scenario]
    [Category("Category A")]
    [Category("Category B")]
    [ScenarioCategory("Category C")]
    [Label(nameof(It_should_capture_tunit_specific_attributes))]
    public async Task It_should_capture_tunit_specific_attributes()
    {
        Runner.RunScenario(Some_step);

        var result = FeatureRunnerProvider.GetRunnerFor(GetType()).GetFeatureResult();
        await Assert.That(result.Info.Description).IsEqualTo("desc");

        var scenario = GetScenarioResult(nameof(It_should_capture_tunit_specific_attributes));
        await Assert.That(scenario.Info.Categories.ToArray()).IsEquivalentTo(new[]
        {
            "Category A",
            "Category B",
            "Category C",
            "Category D",
            "Category E"
        });
    }

    [Scenario]
    [Label(nameof(It_should_capture_tunit_ignore_assertion))]
    public async Task It_should_capture_tunit_ignore_assertion()
    {
        try
        {
            Runner.RunScenario(Ignored_step);
        }
        catch
        {
        }
        var result = GetScenarioResult(nameof(It_should_capture_tunit_ignore_assertion));
        await Assert.That(result.Status).IsEqualTo(ExecutionStatus.Ignored);
    }

    [Scenario]
    public async Task Runner_should_support_async_void_scenarios()
    {
        var scenario = new AsyncScenario();

        await Assert.That(() => Runner
                .AddSteps(scenario.Async_void_step)
                .AddAsyncSteps(
                    scenario.Assert_finished
                )
                .RunAsync())
            .ThrowsNothing();
    }

    class AsyncScenario
    {
        private bool _finished;

#pragma warning disable TUnit0031
        public async void Async_void_step()
#pragma warning restore TUnit0031
        {
            await Task.Delay(200);
            _finished = true;
        }
        public async Task Assert_finished() => await Assert.That(_finished).IsTrue();
    }

    [Scenario]
    public async Task Runner_should_not_support_async_void_scenarios_if_executed_in_sync_mode()
    {
        await Assert.That(() => Runner.RunScenario(Async_void_step))
            .Throws<InvalidOperationException>()
            .WithMessage("Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps.");
    }

    [Scenario]
    [Arguments("abc")]
    [Arguments("def")]
    public async Task Runner_should_support_parameterized_scenarios_with_value(string value)
    {
        await Runner.RunScenarioAsync(_ => Step_with_parameter(value));
        await Assert.That(ConfiguredLightBddScope.CapturedNotifications).Contains($"SCENARIO: Runner should support parameterized scenarios with value \"{value}\"");
    }

    [Scenario]
    [IgnoreScenario("scenario reason")]
    [Label(nameof(Runner_should_ignore_scenario_with_IgnoreScenarioAttribute))]
    public async Task Runner_should_ignore_scenario_with_IgnoreScenarioAttribute()
    {
        await Assert.That(() => Runner.RunScenario(_ => Some_step()))
            .Throws<SkipTestException>()
            .WithMessage("scenario reason");
        var result = GetScenarioResult(nameof(Runner_should_ignore_scenario_with_IgnoreScenarioAttribute));

        await Assert.That(result.Status).IsEqualTo(ExecutionStatus.Ignored);
        await Assert.That(result.StatusDetails).IsEqualTo("Scenario: scenario reason");
        await Assert.That(result.GetSteps().Single().Status).IsEqualTo(ExecutionStatus.NotRun);
    }

    [Scenario]
    [Label(nameof(Runner_should_ignore_step_with_IgnoreScenarioAttribute))]
    public async Task Runner_should_ignore_step_with_IgnoreScenarioAttribute()
    {
        await Assert.That(() => Runner.RunScenario(_ => Declaratively_ignored_step()))
            .Throws<SkipTestException>()
            .WithMessage("step reason");

        var result = GetScenarioResult(nameof(Runner_should_ignore_step_with_IgnoreScenarioAttribute));

        await Assert.That(result.Status).IsEqualTo(ExecutionStatus.Ignored);
        await Assert.That(result.GetSteps().Single().Status).IsEqualTo(ExecutionStatus.Ignored);
        await Assert.That(result.StatusDetails).IsEqualTo("Step 1: step reason");
    }

    [IgnoreScenario("step reason")]
    private void Declaratively_ignored_step()
    {
    }

    private async Task Step_with_parameter(string value)
    {
        await Assert.That(value).IsNotNull();
        await Assert.That(value.Length).IsEqualTo(3);
    }

    private void Ignored_step()
    {
        Skip.Test("ignored");
    }

    private void Some_step()
    {
    }

    private IScenarioResult GetScenarioResult(string scenarioId)
    {
        return FeatureRunnerProvider.GetRunnerFor(GetType())
            .GetFeatureResult()
            .GetScenarios()
            .Single(s => s.Info.Labels.Contains(scenarioId));
    }

#pragma warning disable TUnit0031
    private async void Async_void_step() => await Task.Delay(200);
#pragma warning restore TUnit0031
}