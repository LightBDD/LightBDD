using System.Linq;
using System.Text.RegularExpressions;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest4.UnitTests
{
    [TestClass]
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

            Assert.AreEqual(ExecutionStatus.Bypassed, result.Status);
            Assert.AreEqual(ExecutionStatus.Bypassed, result.GetSteps().Single().Status);
            StringAssert.Contains(result.StatusDetails, "bypass reason");
        }

        [Scenario]
        [Label(nameof(Runner_should_execute_step_when_bypass_setting_is_false))]
        public void Runner_should_execute_step_when_bypass_setting_is_false()
        {
            _settings.ShouldBypass = false;
            Runner.RunScenario(_ => Conditionally_bypassed_step());
            var result = GetScenarioResult(nameof(Runner_should_execute_step_when_bypass_setting_is_false));

            Assert.AreEqual(ExecutionStatus.Passed, result.Status);
            Assert.AreEqual(ExecutionStatus.Passed, result.GetSteps().Single().Status);
        }

        [Scenario]
        [IgnoreScenarioIf(nameof(TestSettings.ShouldIgnore), "ignore reason")]
        [Label(nameof(Runner_should_ignore_scenario_when_setting_is_true))]
        public void Runner_should_ignore_scenario_when_setting_is_true()
        {
            _settings.ShouldIgnore = true;
            var ex = Assert.ThrowsExactly<AssertInconclusiveException>(() => Runner.RunScenario(_ => Some_step()));
            StringAssert.Matches(ex.Message, new Regex("Assert.Inconclusive .*. ignore reason"));
            var result = GetScenarioResult(nameof(Runner_should_ignore_scenario_when_setting_is_true));

            Assert.AreEqual(ExecutionStatus.Ignored, result.Status);
            StringAssert.Matches(result.StatusDetails, new Regex("Scenario: Assert.Inconclusive .*. ignore reason"));
            Assert.AreEqual(ExecutionStatus.NotRun, result.GetSteps().Single().Status);
        }

        [Scenario]
        [IgnoreScenarioIf(nameof(TestSettings.ShouldIgnore), "ignore reason")]
        [Label(nameof(Runner_should_execute_scenario_when_ignore_setting_is_false))]
        public void Runner_should_execute_scenario_when_ignore_setting_is_false()
        {
            _settings.ShouldIgnore = false;
            Runner.RunScenario(_ => Some_step());
            var result = GetScenarioResult(nameof(Runner_should_execute_scenario_when_ignore_setting_is_false));

            Assert.AreEqual(ExecutionStatus.Passed, result.Status);
            Assert.AreEqual(ExecutionStatus.Passed, result.GetSteps().Single().Status);
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
