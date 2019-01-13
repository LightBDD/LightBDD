using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using Shouldly;

namespace LightBDD.Fixie2.UnitTests
{
    [FeatureDescription("desc")]
    [ScenarioCategory("Category C")]
    public class IntegrationTests : FeatureFixture
    {
        [Scenario]
        [Label(nameof(It_should_capture_scenario_name))]
        public void It_should_capture_scenario_name()
        {
            Runner.RunScenario(Some_step);
            var result = GetScenarioResult(nameof(It_should_capture_scenario_name));
            result.Info.Name.ToString().ShouldBe("It should capture scenario name");
        }

        [Scenario]
        [Label(nameof(It_should_capture_scenario_name_after_await))]
        public async Task It_should_capture_scenario_name_after_await()
        {
            await Task.Yield();
            Runner.RunScenario(Some_step);
            var result = GetScenarioResult(nameof(It_should_capture_scenario_name_after_await));
            result.Info.Name.ToString().ShouldBe("It should capture scenario name after await");
        }

        [Scenario]
        [ScenarioCategory("Category A")]
        [ScenarioCategory("Category B")]
        [Label(nameof(It_should_capture_nunit_specific_attributes))]
        public void It_should_capture_nunit_specific_attributes()
        {
            Runner.RunScenario(Some_step);

            var result = FeatureRunnerProvider.GetRunnerFor(GetType()).GetFeatureResult();
            result.Info.Description.ShouldBe("desc");

            var scenario = GetScenarioResult(nameof(It_should_capture_nunit_specific_attributes));
            scenario.Info.Categories.ShouldBe(new[]
            {
                "Category A",
                "Category B",
                "Category C"
            });
        }

        [Scenario]
        [Label(nameof(It_should_capture_ignore_assertion))]
        public void It_should_capture_ignore_assertion()
        {
            try
            {
                Runner.RunScenario(Ignored_step);
            }
            catch
            {
            }
            var result = GetScenarioResult(nameof(It_should_capture_ignore_assertion));
            result.Status.ShouldBe(ExecutionStatus.Ignored);
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
            Action step2 = () => finished.ShouldBeTrue();
            Should.NotThrow(() => Runner.AddSteps(step1, step2).RunAsync());
        }

        [Scenario]
        public void Runner_should_not_support_async_void_scenarios_if_executed_in_sync_mode()
        {
            Action step = async () => await Task.Delay(200);
            Should.Throw<InvalidOperationException>(() => Runner.RunScenario(step))
                .Message.ShouldBe(
                    "Only steps being completed upon return can be run synchronously (all steps have to return completed task). Consider using Async scenario methods for async Task or async void steps.");
        }

        [Scenario]
        [InlineCase("abc")]
        [InlineCase("def")]
        public void Runner_should_support_parameterized_scenarios_with_value(string value)
        {
            Runner.RunScenario(_ => Step_with_parameter(value));
            ConfiguredLightBddScope.CapturedNotifications.ShouldContain($"SCENARIO: Runner should support parameterized scenarios with value \"{value}\"");
        }

        [Scenario]
        [IgnoreScenario("scenario reason")]
        [Label(nameof(Runner_should_ignore_scenario_with_IgnoreScenarioAttribute))]
        public void Runner_should_ignore_scenario_with_IgnoreScenarioAttribute()
        {
            Should.Throw<IgnoreException>(() => Runner.RunScenario(_ => Some_step()))
                .Message.ShouldBe("scenario reason");

            var result = GetScenarioResult(nameof(Runner_should_ignore_scenario_with_IgnoreScenarioAttribute));

            result.Status.ShouldBe(ExecutionStatus.Ignored);
            result.StatusDetails.ShouldBe("Scenario: scenario reason");
            result.GetSteps().Single().Status.ShouldBe(ExecutionStatus.NotRun);
        }

        [Scenario]
        [Label(nameof(Runner_should_ignore_step_with_IgnoreScenarioAttribute))]
        public void Runner_should_ignore_step_with_IgnoreScenarioAttribute()
        {
            Should.Throw<IgnoreException>(() => Runner.RunScenario(_ => Declaratively_ignored_step()))
                .Message.ShouldBe("step reason");
            var result = GetScenarioResult(nameof(Runner_should_ignore_step_with_IgnoreScenarioAttribute));

            result.Status.ShouldBe( ExecutionStatus.Ignored);
            result.GetSteps().Single().Status.ShouldBe( ExecutionStatus.Ignored);
            result.StatusDetails.ShouldBe( "Step 1: step reason");
        }

        [IgnoreScenario("step reason")]
        private void Declaratively_ignored_step()
        {
        }

        private void Step_with_parameter(string value)
        {
            value.ShouldNotBeNull();
            value.Length.ShouldBe(3);
        }

        private void Ignored_step()
        {
            StepExecution.Current.IgnoreScenario("step reason");
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
