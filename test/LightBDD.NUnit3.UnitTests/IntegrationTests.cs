using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Basic;
using NUnit.Framework;

namespace LightBDD.NUnit3.UnitTests
{
    [TestFixture]
    [Description("desc")]
    [Category("Category D"), ScenarioCategory("Category E")]
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
        [Category("Category A"), Category("Category B"), ScenarioCategory("Category C")]
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
            var finished = false;
            Action step1 = async () =>
            {
                await Task.Delay(200);
                finished = true;
            };
            Action step2 = () => Assert.IsTrue(finished);

            Assert.DoesNotThrowAsync(() => Runner.RunScenarioActionsAsync(step1, step2));
        }

        [Scenario]
        public void Runner_should_not_support_async_void_scenarios_if_executed_in_sync_mode()
        {
            Action step = async () => await Task.Delay(200);
            var ex = Assert.Throws<InvalidOperationException>(() => Runner.RunScenario(step));
            Assert.AreEqual(
                    "Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps.",
                    ex.Message);
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
    }
}
