using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Framework.Formatting;

namespace LightBDD.NUnit3.UnitTests
{
    [TestFixture]
    [Description("desc")]
    [Category("Category D")]
    [ScenarioCategory("Category E")]
    public class IntegrationTests : FeatureFixture
    {
        [Scenario]
        [Label(nameof(It_should_capture_scenario_name))]
        public void It_should_capture_scenario_name()
        {
            Runner.RunScenario(Some_step);
            var result = GetScenarioResult(nameof(It_should_capture_scenario_name));
            Assert.That(result.Info.Name.ToString(), Is.EqualTo("It should capture scenario name"));
        }

        [Scenario]
        [Label(nameof(It_should_capture_scenario_name_after_await))]
        public async Task It_should_capture_scenario_name_after_await()
        {
            await Task.Yield();
            Runner.RunScenario(Some_step);
            var result = GetScenarioResult(nameof(It_should_capture_scenario_name_after_await));
            Assert.That(result.Info.Name.ToString(), Is.EqualTo("It should capture scenario name after await"));
        }

        [Scenario]
        [Category("Category A")]
        [Category("Category B")]
        [ScenarioCategory("Category C")]
        [Label(nameof(It_should_capture_nunit_specific_attributes))]
        public void It_should_capture_nunit_specific_attributes()
        {
            Runner.RunScenario(Some_step);

            var result = FeatureRunnerProvider.GetRunnerFor(GetType()).GetFeatureResult();
            Assert.That(result.Info.Description, Is.EqualTo("desc"));

            var scenario = GetScenarioResult(nameof(It_should_capture_nunit_specific_attributes));
            Assert.That(scenario.Info.Categories.ToArray(), Is.EquivalentTo(new[]
            {
                "Category A",
                "Category B",
                "Category C",
                "Category D",
                "Category E"
            }));
        }

        [Scenario]
        [Label(nameof(It_should_capture_nunit_ignore_assertion))]
        public void It_should_capture_nunit_ignore_assertion()
        {
            try
            {
                Runner.RunScenario(Ignored_step);
            }
            catch
            {
            }
            var result = GetScenarioResult(nameof(It_should_capture_nunit_ignore_assertion));
            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Ignored));
        }

        [Scenario]
        [Label(nameof(It_should_capture_nunit_inconclusive_assertion))]
        public void It_should_capture_nunit_inconclusive_assertion()
        {
            try
            {
                Runner.RunScenario(Inconclusive_step);
            }
            catch
            {
            }
            var result = GetScenarioResult(nameof(It_should_capture_nunit_inconclusive_assertion));
            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Ignored));
        }

        [Test]
        public void Runner_should_throw_meaningful_exception_if_scenario_is_not_run_from_Scenario_attribute()
        {
            Exception ex = Assert.Throws<InvalidOperationException>(() => Runner.RunScenario(Some_step));
            Assert.That(
                ex.Message,
                Is.EqualTo("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and test class deriving from FeatureFixture or with [FeatureFixture] attribute."));
        }

        [Scenario]
        public void Runner_should_support_async_void_scenarios()
        {
            var scenario = new AsyncScenario();

            Assert.DoesNotThrowAsync(() => Runner
                .AddSteps(
                    scenario.Async_void_step,
                    scenario.Assert_finished)
                .RunAsync());
        }

        class AsyncScenario
        {
            private bool _finished;

            public async void Async_void_step()
            {
                await Task.Delay(200);
                _finished = true;
            }
            public void Assert_finished() => Assert.IsTrue(_finished);
        }

        [Scenario]
        public void Runner_should_not_support_async_void_scenarios_if_executed_in_sync_mode()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Runner.RunScenario(Async_void_step));
            Assert.AreEqual(
                    "Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps.",
                    ex.Message);
        }

        [Scenario]
        [TestCase("abc")]
        [TestCase("def")]
        public void Runner_should_support_parameterized_scenarios_with_value([Format("@{0}")]string value)
        {
            Runner.RunScenario(_ => Step_with_parameter(value));
            Assert.That(ConfiguredLightBddScope.CapturedNotifications, Does.Contain($"SCENARIO: Runner should support parameterized scenarios with value \"@{value}\""));
        }

        [Scenario]
        [IgnoreScenario("scenario reason")]
        [Label(nameof(Runner_should_ignore_scenario_with_IgnoreScenarioAttribute))]
        public void Runner_should_ignore_scenario_with_IgnoreScenarioAttribute()
        {
            var ex = Assert.Throws<IgnoreException>(() => Runner.RunScenario(_ => Some_step()));
            Assert.AreEqual("scenario reason", ex.Message);
            var result = GetScenarioResult(nameof(Runner_should_ignore_scenario_with_IgnoreScenarioAttribute));

            Assert.AreEqual(ExecutionStatus.Ignored, result.Status);
            Assert.AreEqual("Scenario: scenario reason", result.StatusDetails);
            Assert.AreEqual(ExecutionStatus.NotRun, result.GetSteps().Single().Status);
        }

        [Scenario]
        [Label(nameof(Runner_should_ignore_step_with_IgnoreScenarioAttribute))]
        public void Runner_should_ignore_step_with_IgnoreScenarioAttribute()
        {
            var ex = Assert.Throws<IgnoreException>(() => Runner.RunScenario(_ => Declaratively_ignored_step()));
            Assert.AreEqual("step reason", ex.Message);
            var result = GetScenarioResult(nameof(Runner_should_ignore_step_with_IgnoreScenarioAttribute));

            Assert.AreEqual(ExecutionStatus.Ignored, result.Status);
            Assert.AreEqual(ExecutionStatus.Ignored, result.GetSteps().Single().Status);
            Assert.AreEqual("Step 1: step reason", result.StatusDetails);
        }

        [IgnoreScenario("step reason")]
        private void Declaratively_ignored_step()
        {
        }

        private void Step_with_parameter(string value)
        {
            Assert.NotNull(value);
            Assert.That(value.Length, Is.EqualTo(3));
        }

        private void Inconclusive_step()
        {
            Assert.Inconclusive();
        }

        private void Ignored_step()
        {
            Assert.Ignore();
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

        private async void Async_void_step() => await Task.Delay(200);
    }
}
