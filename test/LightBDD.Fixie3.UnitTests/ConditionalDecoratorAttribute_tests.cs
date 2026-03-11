using System.Linq;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using Shouldly;

namespace LightBDD.Fixie3.UnitTests
{
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

        private class IgnoreScenarioIfAttribute : IgnoreScenarioIfAttribute<TestSettings>
        {
            public IgnoreScenarioIfAttribute(string settingName, params string[] reasons) : base(settingName, reasons) { }
        }

        [Scenario]
        [Label(nameof(Runner_should_bypass_step_when_setting_is_true))]
        public void Runner_should_bypass_step_when_setting_is_true()
        {
            _settings.ShouldBypass = true;
            Runner.RunScenario(_ => Conditionally_bypassed_step());
            var result = GetScenarioResult(nameof(Runner_should_bypass_step_when_setting_is_true));

            result.Status.ShouldBe(ExecutionStatus.Bypassed);
            result.GetSteps().Single().Status.ShouldBe(ExecutionStatus.Bypassed);
            result.StatusDetails.ShouldContain("bypass reason");
        }

        [Scenario]
        [Label(nameof(Runner_should_execute_step_when_bypass_setting_is_false))]
        public void Runner_should_execute_step_when_bypass_setting_is_false()
        {
            _settings.ShouldBypass = false;
            Runner.RunScenario(_ => Conditionally_bypassed_step());
            var result = GetScenarioResult(nameof(Runner_should_execute_step_when_bypass_setting_is_false));

            result.Status.ShouldBe(ExecutionStatus.Passed);
            result.GetSteps().Single().Status.ShouldBe(ExecutionStatus.Passed);
        }

        [Scenario]
        [IgnoreScenarioIf(nameof(TestSettings.ShouldIgnore), "ignore reason")]
        [Label(nameof(Runner_should_ignore_scenario_when_setting_is_true))]
        public void Runner_should_ignore_scenario_when_setting_is_true()
        {
            _settings.ShouldIgnore = true;
            Should.Throw<IgnoreException>(() => Runner.RunScenario(_ => Some_step()))
                .Message.ShouldBe("ignore reason");
            var result = GetScenarioResult(nameof(Runner_should_ignore_scenario_when_setting_is_true));

            result.Status.ShouldBe(ExecutionStatus.Ignored);
            result.StatusDetails.ShouldBe("Scenario: ignore reason");
            result.GetSteps().Single().Status.ShouldBe(ExecutionStatus.NotRun);
        }

        [Scenario]
        [IgnoreScenarioIf(nameof(TestSettings.ShouldIgnore), "ignore reason")]
        [Label(nameof(Runner_should_execute_scenario_when_ignore_setting_is_false))]
        public void Runner_should_execute_scenario_when_ignore_setting_is_false()
        {
            _settings.ShouldIgnore = false;
            Runner.RunScenario(_ => Some_step());
            var result = GetScenarioResult(nameof(Runner_should_execute_scenario_when_ignore_setting_is_false));

            result.Status.ShouldBe(ExecutionStatus.Passed);
            result.GetSteps().Single().Status.ShouldBe(ExecutionStatus.Passed);
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
}
