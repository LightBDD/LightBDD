using System.Linq;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using NUnit.Framework;

namespace LightBDD.NUnit3.UnitTests
{
    [TestFixture]
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

        [SetUp]
        public void SetUp()
        {
            _settings = new TestSettings();
        }

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

            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Bypassed));
            Assert.That(result.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Bypassed));
            Assert.That(result.StatusDetails, Does.Contain("bypass reason"));
        }

        [Scenario]
        [Label(nameof(Runner_should_execute_step_when_bypass_setting_is_false))]
        public void Runner_should_execute_step_when_bypass_setting_is_false()
        {
            _settings.ShouldBypass = false;
            Runner.RunScenario(_ => Conditionally_bypassed_step());
            var result = GetScenarioResult(nameof(Runner_should_execute_step_when_bypass_setting_is_false));

            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Passed));
            Assert.That(result.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Passed));
        }

        [Scenario]
        [IgnoreScenarioIf(nameof(TestSettings.ShouldIgnore), "ignore reason")]
        [Label(nameof(Runner_should_ignore_scenario_when_setting_is_true))]
        public void Runner_should_ignore_scenario_when_setting_is_true()
        {
            _settings.ShouldIgnore = true;
            var ex = Assert.Throws<IgnoreException>(() => Runner.RunScenario(_ => Some_step()));
            Assert.That(ex.Message, Is.EqualTo("ignore reason"));
            var result = GetScenarioResult(nameof(Runner_should_ignore_scenario_when_setting_is_true));

            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Ignored));
            Assert.That(result.StatusDetails, Is.EqualTo("Scenario: ignore reason"));
            Assert.That(result.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.NotRun));
        }

        [Scenario]
        [IgnoreScenarioIf(nameof(TestSettings.ShouldIgnore), "ignore reason")]
        [Label(nameof(Runner_should_execute_scenario_when_ignore_setting_is_false))]
        public void Runner_should_execute_scenario_when_ignore_setting_is_false()
        {
            _settings.ShouldIgnore = false;
            Runner.RunScenario(_ => Some_step());
            var result = GetScenarioResult(nameof(Runner_should_execute_scenario_when_ignore_setting_is_false));

            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Passed));
            Assert.That(result.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Passed));
        }

        [Scenario]
        [Label(nameof(Runner_should_bypass_step_with_multiple_reasons_joined))]
        public void Runner_should_bypass_step_with_multiple_reasons_joined()
        {
            _settings.ShouldBypass = true;
            Runner.RunScenario(_ => Step_with_multiple_bypass_reasons());
            var result = GetScenarioResult(nameof(Runner_should_bypass_step_with_multiple_reasons_joined));

            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Bypassed));
            Assert.That(result.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Bypassed));
            Assert.That(result.StatusDetails, Does.Contain("reason one; reason two"));
        }

        [Scenario]
        [Label(nameof(Runner_should_continue_subsequent_steps_after_bypass))]
        public void Runner_should_continue_subsequent_steps_after_bypass()
        {
            _settings.ShouldBypass = true;
            Runner.RunScenario(
                _ => Conditionally_bypassed_step(),
                _ => Some_step());
            var result = GetScenarioResult(nameof(Runner_should_continue_subsequent_steps_after_bypass));

            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Bypassed));
            var steps = result.GetSteps().ToArray();
            Assert.That(steps[0].Status, Is.EqualTo(ExecutionStatus.Bypassed));
            Assert.That(steps[1].Status, Is.EqualTo(ExecutionStatus.Passed));
        }

        [Scenario]
        [Label(nameof(Runner_should_execute_step_when_fixture_does_not_implement_IIgnorable))]
        public void Runner_should_execute_step_when_fixture_does_not_implement_IIgnorable()
        {
            // The attribute targets TestSettings, but BypassStepIfAttribute<OtherSettings> won't find IIgnorable<OtherSettings>
            _settings.ShouldBypass = true;
            Runner.RunScenario(_ => Step_with_unmatched_settings_type());
            var result = GetScenarioResult(nameof(Runner_should_execute_step_when_fixture_does_not_implement_IIgnorable));

            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Passed));
            Assert.That(result.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Passed));
        }

        [Scenario]
        [Label(nameof(Runner_should_execute_step_when_setting_property_does_not_exist))]
        public void Runner_should_execute_step_when_setting_property_does_not_exist()
        {
            _settings.ShouldBypass = true;
            Runner.RunScenario(_ => Step_with_nonexistent_property());
            var result = GetScenarioResult(nameof(Runner_should_execute_step_when_setting_property_does_not_exist));

            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Passed));
            Assert.That(result.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Passed));
        }

        [Scenario]
        [IgnoreScenarioIf(nameof(TestSettings.ShouldIgnore), "reason one", "reason two")]
        [Label(nameof(Runner_should_ignore_scenario_with_multiple_reasons_joined))]
        public void Runner_should_ignore_scenario_with_multiple_reasons_joined()
        {
            _settings.ShouldIgnore = true;
            var ex = Assert.Throws<IgnoreException>(() => Runner.RunScenario(_ => Some_step()));
            Assert.That(ex.Message, Is.EqualTo("reason one; reason two"));
        }

        [Scenario]
        [IgnoreScenarioIfAttribute_with_other_settings(nameof(OtherSettings.Flag), "other")]
        [Label(nameof(Runner_should_execute_scenario_when_fixture_does_not_implement_IIgnorable_for_settings_type))]
        public void Runner_should_execute_scenario_when_fixture_does_not_implement_IIgnorable_for_settings_type()
        {
            Runner.RunScenario(_ => Some_step());
            var result = GetScenarioResult(nameof(Runner_should_execute_scenario_when_fixture_does_not_implement_IIgnorable_for_settings_type));

            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Passed));
            Assert.That(result.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Passed));
        }

        [Scenario]
        [IgnoreScenarioIf("NonExistentProperty", "should not ignore")]
        [Label(nameof(Runner_should_execute_scenario_when_ignore_setting_property_does_not_exist))]
        public void Runner_should_execute_scenario_when_ignore_setting_property_does_not_exist()
        {
            _settings.ShouldIgnore = true;
            Runner.RunScenario(_ => Some_step());
            var result = GetScenarioResult(nameof(Runner_should_execute_scenario_when_ignore_setting_property_does_not_exist));

            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Passed));
            Assert.That(result.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Passed));
        }

        [BypassStepIf(nameof(TestSettings.ShouldBypass), "bypass reason")]
        private void Conditionally_bypassed_step() { }

        [BypassStepIf(nameof(TestSettings.ShouldBypass), "reason one", "reason two")]
        private void Step_with_multiple_bypass_reasons() { }

        [BypassStepIfAttribute_with_other_settings(nameof(OtherSettings.Flag), "other")]
        private void Step_with_unmatched_settings_type() { }

        [BypassStepIf("NonExistentProperty", "should not bypass")]
        private void Step_with_nonexistent_property() { }

        private void Some_step() { }

        public class OtherSettings
        {
            public bool Flag { get; set; }
        }

        private class BypassStepIfAttribute_with_other_settings : BypassStepIfAttribute<OtherSettings>
        {
            public BypassStepIfAttribute_with_other_settings(string settingName, params string[] reasons) : base(settingName, reasons) { }
        }

        private class IgnoreScenarioIfAttribute_with_other_settings : IgnoreScenarioIfAttribute<OtherSettings>
        {
            public IgnoreScenarioIfAttribute_with_other_settings(string settingName, params string[] reasons) : base(settingName, reasons) { }
        }

        private IScenarioResult GetScenarioResult(string scenarioId)
        {
            return FeatureRunnerProvider.GetRunnerFor(GetType())
                .GetFeatureResult()
                .GetScenarios()
                .Single(s => s.Info.Labels.Contains(scenarioId));
        }
    }
}
