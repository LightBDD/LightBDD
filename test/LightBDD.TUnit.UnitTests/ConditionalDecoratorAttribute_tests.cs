using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using TUnit.Core.Exceptions;

namespace LightBDD.TUnit.UnitTests;

public class ConditionalDecoratorAttribute_tests : FeatureFixture, IBypassable<ConditionalDecoratorAttribute_tests.TestSettings>, IIgnorable<ConditionalDecoratorAttribute_tests.TestSettings>
{
    public class TestSettings
    {
        public bool ShouldBypass { get; set; }
        public bool ShouldIgnore { get; set; }
    }

    private TestSettings _settings = new();
    public TestSettings BypassSettings => _settings;
    public TestSettings IgnoreSettings => _settings;

    private class BypassStepIfAttribute : BypassStepIfAttribute<TestSettings>
    {
        public BypassStepIfAttribute(string settingName, params string[] reasons) : base(settingName, reasons) { }
    }

    internal class IgnoreScenarioIfAttribute : IgnoreScenarioIfAttribute<TestSettings>
    {
        public IgnoreScenarioIfAttribute(string settingName, params string[] reasons) : base(settingName, reasons) { }
    }

    [Scenario]
    [Label(nameof(Runner_should_bypass_step_when_setting_is_true))]
    public async Task Runner_should_bypass_step_when_setting_is_true()
    {
        _settings.ShouldBypass = true;
        Runner.RunScenario(_ => Conditionally_bypassed_step());
        var result = GetScenarioResult(nameof(Runner_should_bypass_step_when_setting_is_true));

        await Assert.That(result.Status).IsEqualTo(ExecutionStatus.Bypassed);
        await Assert.That(result.GetSteps().Single().Status).IsEqualTo(ExecutionStatus.Bypassed);
        await Assert.That(result.StatusDetails).Contains("bypass reason");
    }

    [Scenario]
    [Label(nameof(Runner_should_execute_step_when_bypass_setting_is_false))]
    public async Task Runner_should_execute_step_when_bypass_setting_is_false()
    {
        _settings.ShouldBypass = false;
        Runner.RunScenario(_ => Conditionally_bypassed_step());
        var result = GetScenarioResult(nameof(Runner_should_execute_step_when_bypass_setting_is_false));

        await Assert.That(result.Status).IsEqualTo(ExecutionStatus.Passed);
        await Assert.That(result.GetSteps().Single().Status).IsEqualTo(ExecutionStatus.Passed);
    }

    [Scenario]
    [IgnoreScenarioIf(nameof(TestSettings.ShouldIgnore), "ignore reason")]
    [Label(nameof(Runner_should_ignore_scenario_when_setting_is_true))]
    public async Task Runner_should_ignore_scenario_when_setting_is_true()
    {
        _settings.ShouldIgnore = true;
        await Assert.That(() => Runner.RunScenario(_ => Some_step()))
            .Throws<SkipTestException>()
            .WithMessage("ignore reason");
        var result = GetScenarioResult(nameof(Runner_should_ignore_scenario_when_setting_is_true));

        await Assert.That(result.Status).IsEqualTo(ExecutionStatus.Ignored);
        await Assert.That(result.StatusDetails).IsEqualTo("Scenario: ignore reason");
        await Assert.That(result.GetSteps().Single().Status).IsEqualTo(ExecutionStatus.NotRun);
    }

    [Scenario]
    [IgnoreScenarioIf(nameof(TestSettings.ShouldIgnore), "ignore reason")]
    [Label(nameof(Runner_should_execute_scenario_when_ignore_setting_is_false))]
    public async Task Runner_should_execute_scenario_when_ignore_setting_is_false()
    {
        _settings.ShouldIgnore = false;
        Runner.RunScenario(_ => Some_step());
        var result = GetScenarioResult(nameof(Runner_should_execute_scenario_when_ignore_setting_is_false));

        await Assert.That(result.Status).IsEqualTo(ExecutionStatus.Passed);
        await Assert.That(result.GetSteps().Single().Status).IsEqualTo(ExecutionStatus.Passed);
    }

    [BypassStepIf(nameof(TestSettings.ShouldBypass), "bypass reason")]
    private void Conditionally_bypassed_step() { }

    private void Some_step() { }

    private IScenarioResult GetScenarioResult(string scenarioId)
    {
        return FeatureRunnerProvider.GetRunnerFor(GetType())
            .GetFeatureResult()
            .GetScenarios()
            .Single(s => s.Info.Labels.Contains(scenarioId));
    }
}
